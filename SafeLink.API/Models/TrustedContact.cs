namespace SafeLink.API.Models;

public class TrustedContact
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Relationship { get; set; } = "";   // Onam, Opam, Akam...
    public string AvatarColor { get; set; } = "#7C3AED";

    public User User { get; set; } = null!;
}
