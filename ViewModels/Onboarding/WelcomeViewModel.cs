using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Services;

namespace SafeLink.ViewModels.Onboarding;

public partial class WelcomeViewModel(AuthService auth) : ObservableObject
{
    [ObservableProperty] bool isBusy;
    [ObservableProperty] bool hasError;
    [ObservableProperty] string errorText = "";

    partial void OnErrorTextChanged(string v) => HasError = !string.IsNullOrEmpty(v);

    [RelayCommand]
    async Task Register() => await Shell.Current.GoToAsync("//phone");

    [RelayCommand]
    async Task Login() => await Shell.Current.GoToAsync("//phone");

    // OneID (davlat tizimi) orqali kirish — shaxs darhol tasdiqlanadi,
    // shuning uchun Identity bosqichisiz to'g'ridan-to'g'ri asosiy ekranga o'tadi.
    [RelayCommand]
    async Task OneIdLogin()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorText = "";
        try
        {
            var (ok, _) = await auth.LoginWithOneIdAsync();
            if (ok)
                await Shell.Current.GoToAsync("//main");
            else
                ErrorText = "OneID orqali kirishda xatolik. Qayta urinib ko'ring.";
        }
        catch
        {
            ErrorText = "OneID serveriga ulanib bo'lmadi.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
