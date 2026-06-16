namespace SafeLink.Models;

public class AlertEvent
{
    public int Id { get; set; }
    public DateTime SentAt { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }

    public AlertStatus Status { get; set; } = AlertStatus.Active;
    public int? PoliceEtaMinutes { get; set; }
    public string? AudioFilePath { get; set; }

    public string FormattedTime => SentAt.ToString("HH:mm · dd-MMM yyyy");

    public string FormattedDate => SentAt.Date == DateTime.Today
        ? "Bugun"
        : SentAt.Date == DateTime.Today.AddDays(-1)
            ? "Kecha"
            : SentAt.ToString("dd MMMM yyyy");

    public string StatusLabel => Status switch
    {
        AlertStatus.Resolved  => "Hal qilindi",
        AlertStatus.FalseAlarm => "Yolg'on signal",
        _                      => "Faol",
    };

    public string StatusColor => Status switch
    {
        AlertStatus.Resolved   => "#16A34A",
        AlertStatus.FalseAlarm => "#D97706",
        _                      => "#E11D48",
    };

    public string StatusBgColor => Status switch
    {
        AlertStatus.Resolved   => "#DCFCE7",
        AlertStatus.FalseAlarm => "#FEF3C7",
        _                      => "#FEE2E7",
    };

    public string StatusIcon => Status switch
    {
        AlertStatus.Resolved   => "✓",
        AlertStatus.FalseAlarm => "✕",
        _                      => "!",
    };
}

public enum AlertStatus
{
    Active,
    Resolved,
    FalseAlarm
}
