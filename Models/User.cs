namespace SafeLink.Models;

/// <summary>
/// Ilova foydalanuvchisi — himoyaga muhtoj shaxs.
/// </summary>
public class User
{
    public string FullName { get; set; } = string.Empty;

    // JShShIR — Jismoniy shaxsning shaxsiy identifikatsiya raqami
    public string NationalId { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    // Himoya orderi ma'lumotlari
    public string? ProtectionOrderNumber { get; set; }
    public DateTime? ProtectionOrderExpiry { get; set; }
    public bool HasActiveProtectionOrder =>
        ProtectionOrderNumber != null &&
        ProtectionOrderExpiry.HasValue &&
        ProtectionOrderExpiry.Value > DateTime.Now;

    // Qon guruhi va tibbiy ma'lumotlar
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }

    // Initials uchun (profil avatari)
    public string Initials
    {
        get
        {
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}"
                : FullName.Length > 0 ? FullName[0].ToString() : "?";
        }
    }
}
