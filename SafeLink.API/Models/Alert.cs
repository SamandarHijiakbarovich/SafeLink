namespace SafeLink.API.Models;

public enum AlertStatus { Active, Resolved, FalseAlarm }

public class Alert
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public AlertStatus Status { get; set; } = AlertStatus.Active;

    // Dispatch (IIV) holati: New / EnRoute / OnScene / Closed / False
    public string DispatchStatus { get; set; } = "New";
    public string? AssignedOfficer { get; set; }

    public int? PoliceEtaMinutes { get; set; }
    public string? AudioFilePath { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public User User { get; set; } = null!;
}
