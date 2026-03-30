namespace NotificationService.Scheduling;

public class Reminder
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string Email { get; set; } = null!;
    // Nullable to allow older reminder rows created before we started persisting these details.
    public string? CustomerName { get; set; }
    public string? ServiceName { get; set; }
    public DateTime? AppointmentTime { get; set; }
    public DateTime ReminderTime { get; set; }
    public bool IsSent { get; set; }
}
