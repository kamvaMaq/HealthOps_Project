using HealthOps_Project.Models;
using System.Threading.Tasks;

namespace HealthOps_Project.Services
{
    public interface IEmailService { Task SendEmailAsync(EmailMessage msg); }

    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(EmailMessage msg)
        {
            // Placeholder - integrate SMTP or third-party provider here.
            // For now, just record SentAt.
            msg.SentAt = System.DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
