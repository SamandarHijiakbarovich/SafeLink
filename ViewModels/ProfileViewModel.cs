using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using System.Collections.ObjectModel;

namespace SafeLink.ViewModels;

/// <summary>
/// Profil sahifasi ViewModel.
///
/// Ko'rsatadi:
/// - Foydalanuvchi identifikatsiyasi
/// - Himoya orderi
/// - Brelok holati
/// - Ishonchli kontaktlar ro'yxati
/// - Sozlamalar
/// </summary>
public partial class ProfileViewModel : BaseViewModel
{
    public ProfileViewModel()
    {
        Title = "Profil";
        LoadData();
    }

    // ─── Foydalanuvchi ────────────────────────────────────────
    [ObservableProperty]
    private User _currentUser = new();

    // ─── Brelok ───────────────────────────────────────────────
    [ObservableProperty]
    private BrelokDevice _brelok = new();

    // ─── Kontaktlar ───────────────────────────────────────────
    /// <summary>
    /// ObservableCollection — ro'yxat o'zgarsa UI avtomatik yangilanadi.
    /// </summary>
    public ObservableCollection<TrustedContact> Contacts { get; } = new();

    // ─── Ma'lumot yuklash ─────────────────────────────────────
    private void LoadData()
    {
        // Hozircha hardcoded ma'lumotlar.
        // Keyinroq: SecureStorage yoki API dan olinadi.

        CurrentUser = new User
        {
            FullName = "Madina Karimova",
            NationalId = "4070189610042",
            PhoneNumber = "+998 90 123 45 67",
            ProtectionOrderNumber = "2509-114",
            ProtectionOrderExpiry = new DateTime(2025, 12, 14),
            BloodType = "B(III)+",
        };

        Brelok = new BrelokDevice
        {
            IsConnected = true,
            BatteryLevel = 0.87,
            LastTested = DateTime.Now.AddHours(-1),
            SignalStrength = -55
        };

        Contacts.Clear();
        Contacts.Add(new TrustedContact
        {
            Id = 1, FullName = "Gulnora Karimova",
            Relationship = "Onam",
            PhoneNumber = "+998 90 111 22 33",
            AvatarColor = "#7C3AED"
        });
        Contacts.Add(new TrustedContact
        {
            Id = 2, FullName = "Aziza Toshkentova",
            Relationship = "Opam",
            PhoneNumber = "+998 91 444 55 66",
            AvatarColor = "#0891B2"
        });
        Contacts.Add(new TrustedContact
        {
            Id = 3, FullName = "Botir Karimov",
            Relationship = "Akam",
            PhoneNumber = "+998 93 777 88 99",
            AvatarColor = "#DB2777"
        });
    }

    // ─── Buyruqlar ────────────────────────────────────────────
    [RelayCommand]
    private async Task AddContact()
    {
        // TODO: Kontakt qo'shish sahifasi
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OpenContact(TrustedContact contact)
    {
        // TODO: Kontaktni tahrirlash
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OpenProtectionOrder()
    {
        // TODO: Himoya orderi tafsilotlari
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
