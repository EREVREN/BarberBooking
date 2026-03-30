using BarberBooking.Contracts.Events;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using NotificationService.AI;
using NotificationService.Email;
using NotificationService.Persistence;
using NotificationService.Retry;
using NotificationService.Scheduling;
using System.Text.Json;
                    
namespace NotificationService.Messaging;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DlqPublisher _dlqPublisher;
    private readonly IConsumer<Ignore, string> _consumer;

    public KafkaConsumerService(
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        DlqPublisher dlqPublisher)
    {
        _scopeFactory = scopeFactory;
        _dlqPublisher = dlqPublisher;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "notification-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _consumer.Subscribe("booking.confirmed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);

            var booking = JsonSerializer.Deserialize<BookingConfirmedEvent>(result.Message.Value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (booking == null) continue;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var ai = scope.ServiceProvider.GetRequiredService<IAIMessageGenerator>();

            var exists = await db.ProcessedEvents.AnyAsync(x => x.EventId == booking.AppointmentId);
            if (exists)
            {
                _consumer.Commit(result);
                continue;
            }

            var retryPolicy = RetryPolicies.GetEmailRetryPolicy();

            try
            {
                // 1. Generate AI Confirmation Message
                var body = await ai.GenerateConfirmationMessageAsync(
                    booking.CustomerName,
                    booking.StartTime,
                    booking.ServiceName);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await emailSender.SendAsync(
                        booking.CustomerEmail,
                        "Appointment Confirmed",
                        body);
                });

                // 2. AI-Driven Scheduling: Determine optimal reminder time dynamically
                var reminderTime = await ai.DetermineOptimalReminderTimeAsync(
                    booking.StartTime,
                    booking.ServiceName);

                db.Reminders.Add(new Reminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = booking.AppointmentId,
                    Email = booking.CustomerEmail,
                    CustomerName = booking.CustomerName,
                    ServiceName = booking.ServiceName,
                    AppointmentTime = booking.StartTime,
                    ReminderTime = reminderTime,
                    IsSent = false
                });

                db.ProcessedEvents.Add(new ProcessedEvent
                {
                    EventId = booking.AppointmentId,
                    ProcessedAt = DateTime.UtcNow
                });
                
                await db.SaveChangesAsync();
                _consumer.Commit(result);
            }
            catch (Exception)
            {
                await _dlqPublisher.PublishAsync("booking.confirmed.dlq", booking);
                _consumer.Commit(result);
            }
        }
    }
}
