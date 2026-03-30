using Microsoft.EntityFrameworkCore;
using NotificationService.AI;
using NotificationService.Email;
using NotificationService.Persistence;

namespace NotificationService.Scheduling;

public class ReminderSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReminderSchedulerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var ai = scope.ServiceProvider.GetRequiredService<IAIMessageGenerator>();

            var dueReminders = await db.Reminders
                .Where(r => !r.IsSent && r.ReminderTime <= DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var reminder in dueReminders)
            {
                // Older rows may not have these details populated (schema drift before migration).
                // Avoid crashing the BackgroundService; send a generic reminder instead.
                var customerName = reminder.CustomerName ?? "there";
                var serviceName = reminder.ServiceName ?? "appointment";
                var appointmentTime = reminder.AppointmentTime ?? reminder.ReminderTime;

                // 1. Generate AI-personalized reminder body
                var body = await ai.GenerateReminderMessageAsync(
                    customerName,
                    appointmentTime,
                    serviceName);

                // 2. Send via Email
                await emailSender.SendAsync(
                    reminder.Email,
                    "Appointment Reminder",
                    body);

                reminder.IsSent = true;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
