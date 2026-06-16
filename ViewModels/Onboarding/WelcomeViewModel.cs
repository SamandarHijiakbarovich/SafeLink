using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SafeLink.ViewModels.Onboarding;

public partial class WelcomeViewModel : ObservableObject
{
    [RelayCommand]
    async Task Register() => await Shell.Current.GoToAsync("//onboarding/phone");

    [RelayCommand]
    async Task Login() => await Shell.Current.GoToAsync("//onboarding/phone");
}
