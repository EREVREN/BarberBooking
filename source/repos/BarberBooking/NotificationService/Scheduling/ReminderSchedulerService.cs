using Microsoft.EntityFrameworkCore;
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

            var dueReminders = await db.Reminders
                .Where(r => !r.IsSent && r.ReminderTime <= DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var reminder in dueReminders)
            {
                await emailSender.SendAsync(
                    reminder.Email,
                    "Appointment Reminder",
                    "Your appointment is coming up!");

                reminder.IsSent = true;
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
