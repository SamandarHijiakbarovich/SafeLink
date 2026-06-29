using SafeLink.Models;
using SafeLink.Services.Interfaces;

namespace SafeLink.Services;

public class EmergencyService(IGeolocationService geo, SafeApiClient api, AudioRecordingService audio) : IEmergencyService
{
    public async Task<AlertEvent> SendAlertAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var alert = new AlertEvent
        {
            SentAt = DateTime.Now,
            Status = AlertStatus.Active,
            PoliceEtaMinutes = 4
        };

        // 0. Audio yozishni boshlash (signal davomida atrof ovozi yozib boriladi)
        await audio.StartAsync();

        // 1. GPS (0% → 40%)
        try
        {
            var (lat, lon, address) = await geo.GetCurrentLocationAsync();
            alert.Latitude = lat;
            alert.Longitude = lon;
            alert.Address = address;
        }
        catch
        {
            alert.Address = "Joylashuv aniqlanmadi";
        }
        progress.Report(0.4);

        // 2. Serverga yuborish (40% → 80%)
        var res = await api.PostAsync<AlertResponse>("/alerts/send", new
        {
            latitude = alert.Latitude,
            longitude = alert.Longitude,
            address = alert.Address,
        });

        if (res is not null)
        {
            alert.PoliceEtaMinutes = res.PoliceEta;
        }
        progress.Report(0.8);

        // 3. 102 ga qo'ng'iroq (80% → 100%)
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("102");
        progress.Report(1.0);

        return alert;
    }

    public async Task CancelAlertAsync(AlertEvent alert)
    {
        alert.Status = AlertStatus.FalseAlarm;
        // Audio yozuvni to'xtatib, faylni saqlaymiz
        alert.AudioFilePath = await audio.StopAsync();
    }

    public async Task CallOperatorAsync()
    {
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("102");
        await Task.CompletedTask;
    }

    record AlertResponse(int AlertId, int PoliceEta);
}
