using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services.Interfaces;

namespace SafeLink.ViewModels;

/// <summary>
/// SOS Hold ekrani ViewModel.
///
/// Bu ekran:
/// - 3 soniya sanaydi (Progress: 0 → 1)
/// - Parallel: joylashuv, audio, kontaktlar, 102
/// - "Bekor qilish" tugmasi orqaga qaytaradi
/// - 3 soniya o'tgach AlertSentPage ga o'tadi
/// </summary>
public partial class SosHoldViewModel : BaseViewModel
{
    private readonly IEmergencyService _emergency;
    private CancellationTokenSource? _cts;

    public SosHoldViewModel(IEmergencyService emergency)
    {
        _emergency = emergency;
    }

    // ─── Progress (0.0 → 1.0) ─────────────────────────────────
    /// <summary>
    /// 0.0 = boshlanmadi, 1.0 = yuborildi.
    /// XAML dagi ProgressBar va CountdownRing shunga bog'liq.
    /// </summary>
    [ObservableProperty]
    private double _progress;

    /// <summary>
    /// Qolgan soniyalar: 3 → 2 → 1 → 0
    /// </summary>
    public int SecondsLeft => Math.Max(0, (int)Math.Ceiling((1 - Progress) * 3));

    // ─── Holat chizmalari ─────────────────────────────────────
    [ObservableProperty]
    private bool _locationFound;

    [ObservableProperty]
    private bool _audioRecording;

    [ObservableProperty]
    private bool _policeConnecting;

    // ─── Buyruqlar ────────────────────────────────────────────
    [RelayCommand]
    private async Task StartCountdown()
    {
        _cts = new CancellationTokenSource();
        IsBusy = true;

        try
        {
            // UI animation uchun: 3 soniya davomida progress oshadi
            var animTask = AnimateProgressAsync(_cts.Token);

            // Paralel: real xizmatlar ishlaydi
            var progress = new Progress<double>(p =>
            {
                if (p >= 0.4) LocationFound = true;
                if (p >= 0.7) AudioRecording = true;
                if (p >= 0.9) PoliceConnecting = true;
            });

            var alert = await _emergency.SendAlertAsync(progress, _cts.Token);

            await animTask;

            // Muvaffaqiyat → AlertSent sahifasiga o'tish
            // NavigationParameter orqali alert ma'lumotlarini o'tkazamiz
            await Shell.Current.GoToAsync("alertsent",
                new Dictionary<string, object> { ["Alert"] = alert });
        }
        catch (OperationCanceledException)
        {
            // Foydalanuvchi bekor qildi → bosh sahifaga qaytish
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        // CancellationToken orqali barcha vazifalarni to'xtatamiz
        _cts?.Cancel();
        await Shell.Current.GoToAsync("..");
    }

    // ─── Ichki ────────────────────────────────────────────────
    private async Task AnimateProgressAsync(CancellationToken ct)
    {
        // 3000ms = 3 soniya, har 50ms da progress yangilanadi
        const int totalMs = 3000;
        const int stepMs = 50;
        var steps = totalMs / stepMs;

        for (int i = 0; i <= steps; i++)
        {
            ct.ThrowIfCancellationRequested();
            Progress = (double)i / steps;

            // OnPropertyChanged qo'lda chaqiramiz, chunki
            // SecondsLeft Progress dan hisoblanadi (computed property)
            OnPropertyChanged(nameof(SecondsLeft));

            await Task.Delay(stepMs, ct);
        }
    }
}
