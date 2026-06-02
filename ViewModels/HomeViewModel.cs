using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;

namespace SafeLink.ViewModels;

/// <summary>
/// Bosh sahifa ViewModel.
///
/// Bu yerda:
/// - Foydalanuvchi ma'lumotlari ko'rsatiladi
/// - Brelok holati kuzatiladi
/// - SOS tugmasi bosilganda SosHoldPage ga o'tadi
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    // ─── Foydalanuvchi ────────────────────────────────────────
    [ObservableProperty]
    private string _userName = "Madina Karimova";

    [ObservableProperty]
    private bool _hasProtectionOrder = true;

    [ObservableProperty]
    private string _protectionOrderInfo = "14-dekabr 2025-yilgacha · IIB Toshkent";

    // ─── Brelok holati ────────────────────────────────────────
    [ObservableProperty]
    private bool _brelokConnected = true;

    [ObservableProperty]
    private int _brelokBattery = 87;

    // ─── Xabarnoma ────────────────────────────────────────────
    [ObservableProperty]
    private bool _hasNotification = true;

    // ─── Hisobot ─────────────────────────────────────────────
    public string GreetingText => $"Assalomu alaykum,";
    public string BrelokStatus => BrelokConnected
        ? $"Brelok ulangan · {BrelokBattery}%"
        : "Brelok ulanmagan";

    // ─── Buyruqlar (Commands) ──────────────────────────────────

    /// <summary>
    /// SOS tugmasi bosildi → SosHoldPage ga o'tish.
    ///
    /// [RelayCommand] atributi avtomatik:
    /// - TriggerSosCommand property yaratadi
    /// - CanExecute ni boshqaradi
    /// </summary>
    [RelayCommand]
    private async Task TriggerSos()
    {
        // Shell navigation — AppShell.xaml da ro'yxatdan o'tgan route
        await Shell.Current.GoToAsync("soshold");
    }

    [RelayCommand]
    private async Task OpenProfile()
    {
        await Shell.Current.GoToAsync("profile");
    }

    [RelayCommand]
    private async Task OpenNotifications()
    {
        // TODO: Xabarnomalar sahifasi
        await Task.CompletedTask;
    }
}
