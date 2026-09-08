namespace SafeLink.Officer.Models;

/// <summary>Dispatch hodisasi — favqulodda chaqiriq (xodim ko'rinishi).</summary>
public class Incident
{
    public int Id { get; set; }
    public string CitizenName { get; set; } = "";
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string DispatchStatus { get; set; } = "New";
    public string? AssignedOfficer { get; set; }
    public DateTime SentAt { get; set; }
    public int? PoliceEtaMinutes { get; set; }

    public string IdLabel => $"#UZ-{SentAt:yyMM}-{Id:0000}";
    public string FormattedTime => SentAt.ToLocalTime().ToString("HH:mm");
    public string Eta => PoliceEtaMinutes is { } e && DispatchStatus is "New" or "EnRoute" ? $"{e} daq" : "—";

    public string Tag => DispatchStatus switch
    {
        "EnRoute" => "YO'LDA",
        "OnScene" => "JOYDA",
        "Closed"  => "YAKUNLANDI",
        "False"   => "YOLG'ON",
        _         => "YANGI",
    };

    public string TagColor => DispatchStatus switch
    {
        "EnRoute" => "#2563EB",
        "OnScene" => "#D97706",
        "Closed"  => "#16A34A",
        "False"   => "#64748B",
        _         => "#E11D48",
    };

    public string TagBg => DispatchStatus switch
    {
        "EnRoute" => "#DBEAFE",
        "OnScene" => "#FEF3C7",
        "Closed"  => "#DCFCE7",
        "False"   => "#F0F3F9",
        _         => "#FEE2E7",
    };

    public bool IsNew => DispatchStatus == "New";
    public string CitizenInitials
    {
        get
        {
            var p = CitizenName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return p.Length >= 2 ? $"{p[0][0]}{p[1][0]}" : CitizenName.Length > 0 ? CitizenName[0].ToString() : "?";
        }
    }
}

public class OfficerStats
{
    public int TodayCalls { get; set; }
    public string AvgResponse { get; set; } = "—";
    public int ActivePatrols { get; set; }
    public int Completed { get; set; }
}

public class OfficerInfo
{
    public string ServiceId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public string Token { get; set; } = "";
    public string Initials
    {
        get
        {
            var p = Name.Replace(".", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return p.Length >= 2 ? $"{p[0][0]}{p[^1][0]}" : "JS";
        }
    }
}
