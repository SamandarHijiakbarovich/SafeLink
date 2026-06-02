using SafeLink.Models;
using SafeLink.Services.Interfaces;

namespace SafeLink.Services;

/// <summary>
/// SOS signalni boshqaruvchi asosiy xizmat.
///
/// Qanday ishlaydi:
/// 1. Foydalanuvchi SOS tugmasini 3 soniya bosib turadi
/// 2. Bu xizmat parallel ravishda:
///    - GPS joylashuvni aniqlaydi
///    - Audio yozuvni boshlaydi
///    - Ishonchli kontaktlarga SMS/push yuboradi
///    - 102 ga murojaat qiladi
/// 3. AlertEvent qaytaradi (tarix uchun saqlanadi)
/// </summary>
public class EmergencyService : IEmergencyService
{
    private readonly IGeolocationService _geo;

    public EmergencyService(IGeolocationService geo)
    {
        _geo = geo;
    }

    public async Task<AlertEvent> SendAlertAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        // 3 soniya davomida progress yangilanadi (UI uchun)
        // Real ilovada bu yerda parallel vazifalar ishlaydi

        var alert = new AlertEvent
        {
            SentAt = DateTime.Now,
            Status = AlertStatus.Active,
            PoliceEtaMinutes = 4
        };

        // Qadam 1: Joylashuvni aniqlash (0% → 40%)
        try
        {
            var (lat, lon, address) = await _geo.GetCurrentLocationAsync();
            alert.Latitude = lat;
            alert.Longitude = lon;
            alert.Address = address;
        }
        catch
        {
            // GPS ishlamasa default qiymat
            alert.Address = "Joylashuv aniqlanmadi";
        }

        progress.Report(0.4);

        // Qadam 2: Kontaktlarga xabar yuborish (40% → 70%)
        await NotifyContactsAsync(alert, cancellationToken);
        progress.Report(0.7);

        // Qadam 3: 102 ga murojaat (70% → 100%)
        await NotifyEmergencyServicesAsync(alert, cancellationToken);
        progress.Report(1.0);

        return alert;
    }

    public async Task CancelAlertAsync(AlertEvent alert)
    {
        alert.Status = AlertStatus.FalseAlarm;
        // Real ilovada: 102 va kontaktlarga bekor qilish xabari yuboriladi
        await Task.CompletedTask;
    }

    public async Task CallOperatorAsync()
    {
        // 102 raqamini chaqirish
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("102");

        await Task.CompletedTask;
    }

    // ─── Ichki yordamchi metodlar ─────────────────────────────

    private async Task NotifyContactsAsync(AlertEvent alert, CancellationToken ct)
    {
        // TODO: SMS API yoki push notification yuborish
        // Hozircha simulyatsiya
        await Task.Delay(300, ct);
    }

    private async Task NotifyEmergencyServicesAsync(AlertEvent alert, CancellationToken ct)
    {
        // TODO: 102 API integratsiyasi
        await Task.Delay(300, ct);
    }
}
