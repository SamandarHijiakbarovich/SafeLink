using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SafeLink.ViewModels.Onboarding;

public partial class CompleteViewModel : ObservableObject
{
    [RelayCommand]
    async Task PairDevice() => await Shell.Current.GoToAsync("//main");

    [RelayCommand]
    async Task Skip() => await Shell.Current.GoToAsync("//main");
}
