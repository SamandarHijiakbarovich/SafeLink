namespace SafeLink.Models;

/// <summary>
/// Ishonchli kontakt — SOS signal ketganda xabardor qilinadigan shaxs.
/// </summary>
public class TrustedContact
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty; // Onam, Opam, Akam...
    public string PhoneNumber { get; set; } = string.Empty;

    // Avatar rangi (har bir kontakt uchun turli rang)
    public string AvatarColor { get; set; } = "#7C3AED";

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
