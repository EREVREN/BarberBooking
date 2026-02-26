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

        Console.WriteLine("Consumer started");
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _consumer.Subscribe("booking.confirmed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Waiting for message...");
            var result = _consumer.Consume(stoppingToken);

            //var booking = JsonSerializer.Deserialize<BookingConfirmedEvent>(result.Message.Value);
                
            var booking = JsonSerializer.Deserialize<BookingConfirmedEvent>(result.Message.Value, new JsonSerializerOptions
             {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
             }); 


            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            // Idempotency check
            var exists = await db.ProcessedEvents
                .AnyAsync(x => x.EventId == booking.AppointmentId);

            if (exists)
            {
                _consumer.Commit(result);
                continue;
            }

            var retryPolicy = RetryPolicies.GetEmailRetryPolicy();

            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await emailSender.SendAsync(
                        booking.CustomerEmail,
                        "Appointment Confirmed",
                        $"Your appointment at {booking.StartTime} is confirmed.");
                });
                Console.WriteLine("Message received!");
                db.ProcessedEvents.Add(new ProcessedEvent
                {
                    EventId = booking.AppointmentId,
                    ProcessedAt = DateTime.UtcNow
                });

                // Reminder time calculation (for instance 1 hour before)
                var reminderTime = booking.StartTime.AddHours(-1);

                db.Reminders.Add(new Reminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = booking.AppointmentId,
                    Email = booking.CustomerEmail,
                    ReminderTime = reminderTime,
                    IsSent = false
                });
                
                Console.WriteLine("Saving changes to the database...");
                await db.SaveChangesAsync();
                Console.WriteLine("Changes saved successfully.");
                
                _consumer.Commit(result);
/*
                var ai = scope.ServiceProvider.GetRequiredService<IAIMessageGenerator>();

                var body = await ai.GenerateConfirmationMessageAsync(
                    booking.CustomerName,
                    booking.StartTime,
                    booking.ServiceName);

                await emailSender.SendAsync(
                    booking.CustomerEmail,
                    "Appointment Confirmed",
                    body);
*/
            }
            catch (Exception)
            {
                await _dlqPublisher.PublishAsync(
                    "booking.confirmed.dlq",
                    booking);

                _consumer.Commit(result);
            }
        }
    }
}

// Future:
// + Send Email
// - Generate AI Reminder
// - Publish booking.reminder event