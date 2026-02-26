using MailKit.Net.Smtp;
using MimeKit;

namespace NotificationService.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("BarberBooking", "noreply@barber.local"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("localhost", 1025, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
