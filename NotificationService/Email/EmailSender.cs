using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendAsync(string to, string subject, string body)
        {
            Console.WriteLine($"Email sent to {to}");
            return Task.CompletedTask;
        }
    }

}
