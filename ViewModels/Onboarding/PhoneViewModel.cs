using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Services;

namespace SafeLink.ViewModels.Onboarding;

public partial class PhoneViewModel(AuthService auth) : ObservableObject
{
    [ObservableProperty] string phone = "";
    [ObservableProperty] string otpCode = "";
    [ObservableProperty] bool otpSent;
    [ObservableProperty] bool isBusy;
    [ObservableProperty] string errorText = "";
    [ObservableProperty] int countdown = 60;

    [RelayCommand]
    async Task SendOtp()
    {
        if (string.IsNullOrWhiteSpace(Phone)) return;
        IsBusy = true;
        ErrorText = "";
        var ok = await auth.SendOtpAsync("+998" + Phone.Trim());
        IsBusy = false;
        if (ok)
        {
            OtpSent = true;
            StartCountdown();
        }
        else ErrorText = "SMS yuborishda xato. Qayta urinib ko'ring.";
    }

    [RelayCommand]
    async Task VerifyOtp()
    {
        if (OtpCode.Length < 6) return;
        IsBusy = true;
        ErrorText = "";
        var (success, isNew) = await auth.VerifyOtpAsync("+998" + Phone.Trim(), OtpCode);
        IsBusy = false;
        if (success)
            await Shell.Current.GoToAsync(isNew ? "//onboarding/identity" : "//main");
        else
            ErrorText = "Kod noto'g'ri yoki muddati o'tgan.";
    }

    void StartCountdown()
    {
        Countdown = 60;
        var timer = Dispatcher.GetForCurrentThread()!;
        Task.Run(async () =>
        {
            while (Countdown > 0)
            {
                await Task.Delay(1000);
                timer.Dispatch(() => Countdown--);
            }
        });
    }
}
