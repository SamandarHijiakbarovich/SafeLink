namespace SafeLink.Models;

/// <summary>
/// SOS signal hodisasi — tarix uchun saqlanadi.
/// </summary>
public class AlertEvent
{
    public int Id { get; set; }
    public DateTime SentAt { get; set; }

    // Joylashuv
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }

    // Holat
    public AlertStatus Status { get; set; } = AlertStatus.Active;

    // Politsiya kelish vaqti (daqiqada)
    public int? PoliceEtaMinutes { get; set; }

    // Audio yozuv fayl yo'li
    public string? AudioFilePath { get; set; }

    public string FormattedTime => SentAt.ToString("HH:mm · dd-MMMM");
}

public enum AlertStatus
{
    Active,      // Faol — yordam yo'lda
    Resolved,    // Hal qilindi
    FalseAlarm   // Yolg'on signal
}
