using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Officer.Services;

namespace SafeLink.Officer.ViewModels;

public partial class LoginViewModel(OfficerAuthService auth) : ObservableObject
{
    [ObservableProperty] string serviceId = "IIB-TSH-04287";
    [ObservableProperty] string password = "";
    [ObservableProperty] bool trustDevice = true;
    [ObservableProperty] bool isBusy;
    [ObservableProperty] bool hasError;
    [ObservableProperty] string errorText = "";

    partial void OnErrorTextChanged(string v) => HasError = !string.IsNullOrEmpty(v);

    [RelayCommand]
    async Task Login()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(ServiceId))
        {
            ErrorText = "Xizmat ID kiriting.";
            return;
        }

        IsBusy = true;
        ErrorText = "";
        try
        {
            if (await auth.LoginAsync(ServiceId.Trim(), Password))
                await Shell.Current.GoToAsync("//dashboard");
            else
                ErrorText = "Kirishda xatolik. Ma'lumotlarni tekshiring.";
        }
        catch
        {
            ErrorText = "Serverga ulanib bo'lmadi.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
