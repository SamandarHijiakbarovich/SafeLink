namespace SafeLink.API.Models;

public class User
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? NationalId { get; set; }               // JShShIR (ro'yxatdan o'tishda null, keyin to'ldiriladi)
    public string? ProtectionOrderNumber { get; set; }
    public DateTime? ProtectionOrderExpiry { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TrustedContact> TrustedContacts { get; set; } = [];
    public List<Alert> Alerts { get; set; } = [];
}
