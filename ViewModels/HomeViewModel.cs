using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;

namespace SafeLink.ViewModels;

/// <summary>
/// Bosh sahifa ViewModel.
///
/// Bu yerda:
/// - Foydalanuvchi ma'lumotlari ko'rsatiladi
/// - Brelok holati kuzatiladi
/// - SOS tugmasi bosilganda SosHoldPage ga o'tadi
/// </summary>
public partial class HomeViewModel(SafeApiClient api) : BaseViewModel
{
    // ─── Foydalanuvchi ────────────────────────────────────────
    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private bool _hasProtectionOrder;

    [ObservableProperty]
    private string _protectionOrderInfo = "";

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

    // ─── Ma'lumotlarni yuklash ────────────────────────────────
    /// <summary>
    /// Bosh sahifa ochilganda haqiqiy foydalanuvchi ma'lumotini API'dan yuklaydi.
    /// </summary>
    public async Task LoadAsync()
    {
        try
        {
            var p = await api.GetAsync<HomeProfile>("/profile");
            if (p is null) return;

            UserName = string.IsNullOrWhiteSpace(p.FullName) ? "Foydalanuvchi" : p.FullName;

            HasProtectionOrder = !string.IsNullOrWhiteSpace(p.ProtectionOrderNumber);
            if (HasProtectionOrder)
            {
                var muddat = p.ProtectionOrderExpiry is { } d
                    ? $"{d:dd-MMMM yyyy}-gacha"
                    : "muddatsiz";
                ProtectionOrderInfo = $"№ {p.ProtectionOrderNumber} · {muddat}";
            }
            else
            {
                ProtectionOrderInfo = "Himoya orderi kiritilmagan";
            }
        }
        catch
        {
            // Tarmoq xatosi — mavjud holatni saqlab qolamiz (soxta ma'lumot ko'rsatmaymiz)
        }
    }

    record HomeProfile(
        string FullName, string? NationalId, string PhoneNumber,
        string? ProtectionOrderNumber, DateTime? ProtectionOrderExpiry,
        string? BloodType, string? Allergies);

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
        var page = Shell.Current?.CurrentPage;
        if (page is not null)
            await page.DisplayAlert("Xabarnomalar", "Hozircha yangi xabarnoma yo'q.", "OK");
    }
}
