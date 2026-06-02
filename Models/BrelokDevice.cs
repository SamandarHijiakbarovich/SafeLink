namespace SafeLink.Models;

/// <summary>
/// SafeLink Brelok SL-100 — fizik SOS tugma qurilmasi.
/// Bluetooth LE orqali ulangan.
/// </summary>
public class BrelokDevice
{
    public string Name { get; set; } = "SafeLink SL-100";
    public string? MacAddress { get; set; }

    // Bluetooth holati
    public bool IsConnected { get; set; }
    public int SignalStrength { get; set; } // dBm

    // Batareya (0.0 dan 1.0 gacha)
    public double BatteryLevel { get; set; }
    public int BatteryPercent => (int)(BatteryLevel * 100);

    // Oxirgi sinov
    public DateTime? LastTested { get; set; }

    public string ConnectionStatus => IsConnected ? "Ulangan" : "Ulanmagan";
    public string SignalDescription => SignalStrength switch
    {
        > -60 => "Kuchli",
        > -75 => "O'rtacha",
        > -90 => "Zaif",
        _ => "Signal yo'q"
    };
}
