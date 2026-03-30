using Microsoft.EntityFrameworkCore;
using NotificationService;
using NotificationService.AI;
using NotificationService.Email;
using NotificationService.Messaging;
using NotificationService.Persistence;
using NotificationService.Scheduling;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("Default"),
                ServerVersion.AutoDetect(
                    builder.Configuration.GetConnectionString("Default")
                )));

builder.Services.AddHttpClient<IAIMessageGenerator, LLMMessageGenerator>(client =>
{
    var timeoutSeconds = builder.Configuration.GetValue<int?>("LLM:HttpTimeoutSeconds") ?? 120;
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddSingleton<DlqPublisher>();
builder.Services.AddHostedService<ReminderSchedulerService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

var host = builder.Build();

// Apply any pending EF migrations at startup so schema stays aligned with the model.
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.Migrate();
}
host.Run();
