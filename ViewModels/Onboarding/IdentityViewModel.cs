using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Services;

namespace SafeLink.ViewModels.Onboarding;

public partial class IdentityViewModel(SafeApiClient api) : ObservableObject
{
    [ObservableProperty] string fullName = "";
    [ObservableProperty] string nationalId = "";
    [ObservableProperty] bool isBusy;
    [ObservableProperty] string errorText = "";

    public bool CanContinue => FullName.Length > 2 && NationalId.Length == 14;

    partial void OnFullNameChanged(string v) => OnPropertyChanged(nameof(CanContinue));
    partial void OnNationalIdChanged(string v) => OnPropertyChanged(nameof(CanContinue));

    [RelayCommand]
    async Task Continue()
    {
        IsBusy = true;
        await api.PostAsync<object>("/profile", new { fullName = FullName, nationalId = NationalId });
        IsBusy = false;
        await Shell.Current.GoToAsync("//onboarding/order");
    }
}
