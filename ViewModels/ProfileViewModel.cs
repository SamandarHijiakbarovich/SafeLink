using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;
using System.Collections.ObjectModel;

namespace SafeLink.ViewModels;

public partial class ProfileViewModel(SafeApiClient api) : BaseViewModel
{
    [ObservableProperty] User currentUser = new();
    [ObservableProperty] BrelokDevice brelok = new();
    [ObservableProperty] bool isLoading;

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
            LoadDemoData();
        }
        finally
        {
            IsLoading = false;
        }

        // Brelok holati (lokal, Bluetooth orqali)
        Brelok = new BrelokDevice { IsConnected = true, BatteryLevel = 0.87, LastTested = DateTime.Now.AddHours(-1) };
    }

    void LoadDemoData()
    {
        CurrentUser = new User
        {
            FullName = "Madina Karimova", NationalId = "4070189610042",
            PhoneNumber = "+998 90 123 45 67", ProtectionOrderNumber = "2509-114",
            ProtectionOrderExpiry = new DateTime(2025, 12, 14), BloodType = "B(III)+",
        };
        Contacts.Clear();
        Contacts.Add(new TrustedContact { Id = 1, FullName = "Gulnora Karimova", Relationship = "Onam", PhoneNumber = "+998 90 111 22 33", AvatarColor = "#7C3AED" });
        Contacts.Add(new TrustedContact { Id = 2, FullName = "Aziza Toshkentova", Relationship = "Opam", PhoneNumber = "+998 91 444 55 66", AvatarColor = "#0891B2" });
        Contacts.Add(new TrustedContact { Id = 3, FullName = "Botir Karimov",     Relationship = "Akam", PhoneNumber = "+998 93 777 88 99", AvatarColor = "#DB2777" });
    }

    [RelayCommand] async Task AddContact()    => await Task.CompletedTask;
    [RelayCommand] async Task OpenContact(TrustedContact c) => await Task.CompletedTask;
    [RelayCommand] async Task OpenProtectionOrder()  => await Task.CompletedTask;
    [RelayCommand] async Task GoBack() => await Shell.Current.GoToAsync("..");

    record ProfileResponse(
        string FullName, string NationalId, string PhoneNumber,
        string? ProtectionOrderNumber, DateTime? ProtectionOrderExpiry,
        string? BloodType, string? Allergies,
        List<ContactDto>? TrustedContacts);
    record ContactDto(int Id, string FullName, string PhoneNumber, string Relationship, string AvatarColor);
}
