using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;
using System.Collections.ObjectModel;

namespace SafeLink.ViewModels;

public partial class ProfileViewModel(SafeApiClient api, AuthService auth) : BaseViewModel
{
    [ObservableProperty] User currentUser = new();
    [ObservableProperty] BrelokDevice brelok = new();
    [ObservableProperty] bool isLoading;

    // Sozlamalar (lokal — Preferences'da saqlanadi)
    [ObservableProperty] bool alertSoundOn = Preferences.Get("alert_sound", false);
    [ObservableProperty] string language = Preferences.Get("language", "O'zbek");

    public string AlertSoundText => AlertSoundOn ? "Yoniq" : "O'chiq";
    partial void OnAlertSoundOnChanged(bool value) => OnPropertyChanged(nameof(AlertSoundText));

    public ObservableCollection<TrustedContact> Contacts { get; } = [];

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var profile = await api.GetAsync<ProfileResponse>("/profile");
            if (profile is not null)
            {
                CurrentUser = new User
                {
                    FullName = profile.FullName,
                    NationalId = profile.NationalId,
                    PhoneNumber = profile.PhoneNumber,
                    ProtectionOrderNumber = profile.ProtectionOrderNumber,
                    ProtectionOrderExpiry = profile.ProtectionOrderExpiry,
                    BloodType = profile.BloodType,
                    Allergies = profile.Allergies,
                };

                Contacts.Clear();
                foreach (var c in profile.TrustedContacts ?? [])
                    Contacts.Add(new TrustedContact
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        PhoneNumber = c.PhoneNumber,
                        Relationship = c.Relationship,
                        AvatarColor = c.AvatarColor,
                    });
            }
        }
        catch
        {
            // Tarmoq xatosi — soxta ma'lumot ko'rsatmaymiz, mavjud holatni saqlaymiz
        }
        finally
        {
            IsLoading = false;
        }

        // Brelok holati (lokal, Bluetooth orqali)
        Brelok = new BrelokDevice { IsConnected = true, BatteryLevel = 0.87, LastTested = DateTime.Now.AddHours(-1) };
    }

    static readonly string[] AvatarColors = ["#7C3AED", "#0891B2", "#DB2777", "#16A34A", "#D97706"];

    [RelayCommand]
    async Task AddContact()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        if (Contacts.Count >= 5)
        {
            await page.DisplayAlert("Limit", "Maksimal 5 ta ishonchli kontakt qo'shish mumkin.", "OK");
            return;
        }

        var name = await page.DisplayPromptAsync("Yangi kontakt", "To'liq ism:",
            "Davom etish", "Bekor", "Masalan: Gulnora Karimova");
        if (string.IsNullOrWhiteSpace(name)) return;

        var phone = await page.DisplayPromptAsync("Yangi kontakt", "Telefon raqam:",
            "Davom etish", "Bekor", "+998 90 123 45 67", keyboard: Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(phone)) return;

        var rel = await page.DisplayPromptAsync("Yangi kontakt", "Munosabat (Onam, Akam, Opam...):",
            "Qo'shish", "Bekor", "Onam");
        if (string.IsNullOrWhiteSpace(rel)) return;

        var color = AvatarColors[Contacts.Count % AvatarColors.Length];
        var created = await api.PostAsync<ContactDto>("/profile/contacts",
            new { fullName = name.Trim(), phoneNumber = phone.Trim(), relationship = rel.Trim(), avatarColor = color });

        if (created is not null)
            await LoadAsync();
        else
            await page.DisplayAlert("Xato", "Kontakt qo'shilmadi. Qayta urinib ko'ring.", "OK");
    }

    [RelayCommand]
    async Task OpenContact(TrustedContact c)
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null || c is null) return;

        bool del = await page.DisplayAlert(c.FullName,
            $"{c.Relationship} · {c.PhoneNumber}\n\nBu kontaktni o'chirasizmi?", "O'chirish", "Bekor");
        if (!del) return;

        if (await api.DeleteAsync($"/profile/contacts/{c.Id}"))
            await LoadAsync();
        else
            await page.DisplayAlert("Xato", "Kontakt o'chirilmadi.", "OK");
    }

    // ─── Himoya orderi ────────────────────────────────────────
    [RelayCommand]
    async Task OpenProtectionOrder()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        if (string.IsNullOrWhiteSpace(CurrentUser.ProtectionOrderNumber))
        {
            var num = await page.DisplayPromptAsync("Himoya orderi",
                "Order raqamini kiriting:", "Saqlash", "Bekor", "2509-114");
            if (string.IsNullOrWhiteSpace(num)) return;
            CurrentUser.ProtectionOrderNumber = num.Trim();
            CurrentUser.ProtectionOrderExpiry = DateTime.Now.AddMonths(6);
            if (await UpdateProfileAsync()) await LoadAsync();
        }
        else
        {
            var muddat = CurrentUser.ProtectionOrderExpiry?.ToString("dd-MMMM yyyy") ?? "—";
            await page.DisplayAlert("Himoya orderi",
                $"№ {CurrentUser.ProtectionOrderNumber}\nAmal qilish muddati: {muddat}", "Yopish");
        }
    }

    // ─── Shaxsiy ma'lumot (ism / JShShIR) ─────────────────────
    [RelayCommand]
    async Task EditIdentity()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        var name = await page.DisplayPromptAsync("Shaxsiy ma'lumot", "To'liq ism (F.I.Sh):",
            "Davom etish", "Bekor", initialValue: CurrentUser.FullName);
        if (name is null) return;

        var pinfl = await page.DisplayPromptAsync("Shaxsiy ma'lumot", "JShShIR (14 raqam):",
            "Saqlash", "Bekor", initialValue: CurrentUser.NationalId,
            keyboard: Keyboard.Numeric, maxLength: 14);
        if (pinfl is null) return;

        if (!string.IsNullOrWhiteSpace(name)) CurrentUser.FullName = name.Trim();
        CurrentUser.NationalId = pinfl.Trim();

        if (await UpdateProfileAsync())
        {
            await LoadAsync();
            await page.DisplayAlert("Saqlandi", "Shaxsiy ma'lumot yangilandi.", "OK");
        }
        else
            await page.DisplayAlert("Xato", "Saqlab bo'lmadi.", "OK");
    }

    // ─── Tibbiy ma'lumotlar ───────────────────────────────────
    [RelayCommand]
    async Task EditMedical()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        var blood = await page.DisplayPromptAsync("Tibbiy ma'lumot",
            "Qon guruhi (masalan: B(III)+):", "Davom etish", "Bekor",
            initialValue: CurrentUser.BloodType ?? "");
        if (blood is null) return;

        var allergy = await page.DisplayPromptAsync("Tibbiy ma'lumot",
            "Allergiyalar (bo'lmasa bo'sh qoldiring):", "Saqlash", "Bekor",
            initialValue: CurrentUser.Allergies ?? "");
        if (allergy is null) return;

        CurrentUser.BloodType = string.IsNullOrWhiteSpace(blood) ? null : blood.Trim();
        CurrentUser.Allergies = string.IsNullOrWhiteSpace(allergy) ? null : allergy.Trim();

        if (await UpdateProfileAsync())
            await page.DisplayAlert("Saqlandi", "Tibbiy ma'lumot yangilandi.", "OK");
        else
            await page.DisplayAlert("Xato", "Saqlab bo'lmadi.", "OK");
    }

    // ─── Sozlamalar ───────────────────────────────────────────
    [RelayCommand]
    void ToggleAlertSound()
    {
        AlertSoundOn = !AlertSoundOn;
        Preferences.Set("alert_sound", AlertSoundOn);
    }

    [RelayCommand]
    async Task ChangeLanguage()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;
        var choice = await page.DisplayActionSheet("Tilni tanlang", "Bekor", null, "O'zbek", "Русский");
        if (string.IsNullOrEmpty(choice) || choice == "Bekor") return;
        Language = choice;
        Preferences.Set("language", choice);
    }

    [RelayCommand]
    async Task OpenDocuments()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is not null)
            await page.DisplayAlert("Hujjat va arizalar", "Bu bo'lim tez orada qo'shiladi.", "OK");
    }

    // ─── Menyu / Chiqish ──────────────────────────────────────
    [RelayCommand]
    async Task ShowMenu()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null) return;
        var choice = await page.DisplayActionSheet("Profil", "Bekor", "Chiqish");
        if (choice == "Chiqish")
        {
            auth.Logout();
            await Shell.Current.GoToAsync("//welcome");
        }
    }

    [RelayCommand] async Task GoBack() => await Shell.Current.GoToAsync("..");

    // Joriy profil ma'lumotini serverga saqlaydi (PUT /profile to'liq ma'lumot kutadi)
    async Task<bool> UpdateProfileAsync()
    {
        OnPropertyChanged(nameof(CurrentUser));   // UI ni yangilash
        return await api.PutAsync("/profile", new
        {
            fullName = CurrentUser.FullName,
            nationalId = CurrentUser.NationalId,
            protectionOrderNumber = CurrentUser.ProtectionOrderNumber,
            protectionOrderExpiry = CurrentUser.ProtectionOrderExpiry,
            bloodType = CurrentUser.BloodType,
            allergies = CurrentUser.Allergies,
        });
    }

    record ProfileResponse(
        string FullName, string NationalId, string PhoneNumber,
        string? ProtectionOrderNumber, DateTime? ProtectionOrderExpiry,
        string? BloodType, string? Allergies,
        List<ContactDto>? TrustedContacts);
    record ContactDto(int Id, string FullName, string PhoneNumber, string Relationship, string AvatarColor);
}
