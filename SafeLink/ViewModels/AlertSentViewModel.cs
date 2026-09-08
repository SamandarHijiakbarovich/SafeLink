using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;
using SafeLink.Services.Interfaces;

namespace SafeLink.ViewModels;

[QueryProperty(nameof(Alert), "Alert")]
public partial class AlertSentViewModel : BaseViewModel
{
    private readonly IEmergencyService _emergency;
    private readonly SignalRService _signalR;
    private readonly AuthService _auth;

    public AlertSentViewModel(IEmergencyService emergency, SignalRService signalR, AuthService auth)
    {
        _emergency = emergency;
        _signalR = signalR;
        _auth = auth;
        _signalR.StatusUpdated += OnStatusUpdated;
    }

    [ObservableProperty] private AlertEvent? _alert;

    partial void OnAlertChanged(AlertEvent? value)
    {
        if (value is null) return;
        SentTime = value.FormattedTime;
        Address = value.Address ?? "Manzil aniqlanmoqda...";
        PoliceEta = $"~{value.PoliceEtaMinutes} daqiqa";

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

        _ = ConnectSignalR();
    }

    async Task ConnectSignalR()
    {
        try
        {
            var token = await _auth.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
                await _signalR.ConnectAsync(token, Alert?.Id ?? 0);
        }
        catch { /* SignalR ulana olmasa — foydalanuvchi hali ham himoyada */ }
    }

    void OnStatusUpdated(string status, string? officer)
    {
        PoliceStatusText = status switch
        {
            "EnRoute" => $"\U0001f694 {officer ?? "Politsiya"} yo'lda",
            "OnScene" => $"✅ {officer ?? "Politsiya"} yetib keldi",
            "Closed"  => "✅ Hodisa yopildi",
            _         => "\U0001f6a8 Politsiya qabul qildi",
        };
        HasPoliceStatus = true;
    }

    [ObservableProperty] string mapUrl = "";
    [ObservableProperty] bool hasMap;
    [ObservableProperty] private string _sentTime = "";
    [ObservableProperty] private string _address = "Aniqlanmoqda...";
    [ObservableProperty] private string _policeEta = "~4 daqiqa";
    [ObservableProperty] private bool _isAudioRecording = true;
    [ObservableProperty] private string _audioDuration = "00:00";
    [ObservableProperty] string policeStatusText = "";
    [ObservableProperty] bool hasPoliceStatus;

    [RelayCommand]
    private async Task CallOperator() => await _emergency.CallOperatorAsync();

    [RelayCommand]
    private async Task CancelFalseAlarm()
    {
        if (Alert is null) return;
        var confirm = await Shell.Current.DisplayAlert(
            "Yolg'on signal",
            "Haqiqatan ham bu yolg'on signal edimi?",
            "Ha, bekor qilish", "Yo'q");
        if (!confirm) return;
        await _emergency.CancelAlertAsync(Alert);
        await Shell.Current.GoToAsync("///home");
    }
}
