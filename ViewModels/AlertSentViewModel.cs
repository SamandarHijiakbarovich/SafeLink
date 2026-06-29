using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services.Interfaces;

namespace SafeLink.ViewModels;

/// <summary>
/// "Ogohlantirish yuborildi" ekrani ViewModel.
///
/// Bu ekran:
/// - Xarita ko'rsatadi (joylashuv bilan)
/// - Politsiya ETA ko'rsatadi
/// - Kontaktlar holatini ko'rsatadi
/// - Audio yozuv davom etmoqda
/// - Operator bilan bog'lanish tugmasi
/// - Yolg'on signal → bekor qilish
/// </summary>
[QueryProperty(nameof(Alert), "Alert")]
public partial class AlertSentViewModel : BaseViewModel
{
    private readonly IEmergencyService _emergency;

    public AlertSentViewModel(IEmergencyService emergency)
    {
        _emergency = emergency;
    }

    // ─── Alert ma'lumotlari ───────────────────────────────────
    /// <summary>
    /// [QueryProperty] — Shell navigation orqali kelgan parameter.
    /// SosHoldViewModel GoToAsync("alertsent", {Alert: alert}) deb yuboradi.
    /// </summary>
    [ObservableProperty]
    private AlertEvent? _alert;

    // Alert o'zgarsa — bog'liq property larni yangilash
    partial void OnAlertChanged(AlertEvent? value)
    {
        if (value == null) return;
        SentTime = value.FormattedTime;
        Address = value.Address ?? "Manzil aniqlanmoqda...";
        PoliceEta = $"~{value.PoliceEtaMinutes} daqiqa";

        // OpenStreetMap (API kalit kerak emas) — joylashuvni markerda ko'rsatadi
        if (value.Latitude != 0 || value.Longitude != 0)
        {
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            const double d = 0.004;
            string lat = value.Latitude.ToString(ci), lon = value.Longitude.ToString(ci);
            string bbox = $"{(value.Longitude - d).ToString(ci)},{(value.Latitude - d).ToString(ci)}," +
                          $"{(value.Longitude + d).ToString(ci)},{(value.Latitude + d).ToString(ci)}";
            MapUrl = $"https://www.openstreetmap.org/export/embed.html?bbox={bbox}&layer=mapnik&marker={lat},{lon}";
            HasMap = true;
        }
    }

    // ─── Xarita ───────────────────────────────────────────────
    [ObservableProperty] string mapUrl = "";
    [ObservableProperty] bool hasMap;

    // ─── Ko'rsatiladigan ma'lumotlar ──────────────────────────
    [ObservableProperty]
    private string _sentTime = "21:47 · Bugun";

    [ObservableProperty]
    private string _address = "Aniqlanmoqda...";

    [ObservableProperty]
    private string _policeEta = "~4 daqiqa";

    [ObservableProperty]
    private bool _isAudioRecording = true;

    [ObservableProperty]
    private string _audioDuration = "00:34";

    // ─── Buyruqlar ────────────────────────────────────────────
    [RelayCommand]
    private async Task CallOperator()
    {
        await _emergency.CallOperatorAsync();
    }

    [RelayCommand]
    private async Task CancelFalseAlarm()
    {
        if (Alert == null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Yolg'on signal",
            "Haqiqatan ham bu yolg'on signal edimi? Politsiya va kontaktlar xabardor qilinadi.",
            "Ha, bekor qilish",
            "Yo'q"
        );

        if (!confirm) return;

        await _emergency.CancelAlertAsync(Alert);
        // Bosh sahifaga qaytish (navigation stekni tozalash)
        await Shell.Current.GoToAsync("///home");
    }
}
