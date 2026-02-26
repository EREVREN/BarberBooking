using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSingleton<DlqPublisher>();
builder.Services.AddHostedService<ReminderSchedulerService>();
builder.Services.AddScoped<IAIMessageGenerator, OpenAiMessageGenerator>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
