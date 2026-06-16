using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;
using System.Collections.ObjectModel;

namespace SafeLink.ViewModels.Onboarding;

public partial class ContactsViewModel(SafeApiClient api) : ObservableObject
{
    public ObservableCollection<TrustedContact> Contacts { get; } = [];

    [ObservableProperty] string newName = "";
    [ObservableProperty] string newPhone = "";
    [ObservableProperty] string newRelation = "";
    [ObservableProperty] bool showForm;

    static readonly string[] Colors = ["#7C3AED", "#0891B2", "#DB2777", "#059669", "#D97706"];

    [RelayCommand]
    void OpenForm() => ShowForm = true;

    [RelayCommand]
    async Task AddContact()
    {
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewPhone)) return;
        var contact = new TrustedContact
        {
            FullName = NewName,
            PhoneNumber = NewPhone,
            Relationship = NewRelation,
            AvatarColor = Colors[Contacts.Count % Colors.Length],
        };
        await api.PostAsync<object>("/profile/contacts", new
        {
            fullName = contact.FullName,
            phoneNumber = contact.PhoneNumber,
            relationship = contact.Relationship,
            avatarColor = contact.AvatarColor,
        });
        Contacts.Add(contact);
        NewName = NewPhone = NewRelation = "";
        ShowForm = false;
    }

    [RelayCommand]
    async Task Continue() => await Shell.Current.GoToAsync("//complete");
}
