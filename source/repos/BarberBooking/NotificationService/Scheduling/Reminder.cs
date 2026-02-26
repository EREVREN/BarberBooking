namespace NotificationService.Scheduling;

public class Reminder
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }   // EventId olarak kullanacağız
    
    public string Email { get; set; } = null!;

    public DateTime ReminderTime { get; set; }

    public bool IsSent { get; set; }
}
