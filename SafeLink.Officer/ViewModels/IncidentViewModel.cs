using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Officer.Models;
using SafeLink.Officer.Services;

namespace SafeLink.Officer.ViewModels;

[QueryProperty(nameof(Incident), "Incident")]
public partial class IncidentViewModel(OfficerApiClient api, OfficerAuthService auth) : ObservableObject
{
    [ObservableProperty] Incident? incident;
    [ObservableProperty] string mapUrl = "";
    [ObservableProperty] bool hasMap;
    [ObservableProperty] bool isBusy;
    [ObservableProperty] string primaryActionText = "";
    [ObservableProperty] bool canAct;

    partial void OnIncidentChanged(Incident? value)
    {
        if (value is null) return;

        if (value.Latitude != 0 || value.Longitude != 0)
        {
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            const double d = 0.004;
            string lat = value.Latitude.ToString(ci), lon = value.Longitude.ToString(ci);
            string bbox = $"{(value.Longitude - d).ToString(ci)},{(value.Latitude - d).ToString(ci)}," +
                          $"{(value.Longitude + d).ToString(ci)},{(value.Latitude + d).ToString(ci)}";
            MapUrl = $"https://www.openstreetmap.org/export/embed.html?bbox={bbox}&layer=mapnik&marker={lat},{lon}";
            HasMap = true;
        }
        RefreshAction();
    }

    void RefreshAction()
    {
        (PrimaryActionText, CanAct) = Incident?.DispatchStatus switch
        {
            "New"     => ("Qabul qilish · Yo'lga chiqish", true),
            "EnRoute" => ("Joyga yetib keldim", true),
            "OnScene" => ("Hodisani yakunlash", true),
            _         => ("", false),
        };
        OnPropertyChanged(nameof(Incident));
    }

    [RelayCommand]
    async Task Advance()
    {
        if (Incident is null || IsBusy) return;
        var next = Incident.DispatchStatus switch
        {
            "New" => "EnRoute",
            "EnRoute" => "OnScene",
            "OnScene" => "Closed",
            _ => null,
        };
        if (next is null) return;
        await UpdateStatusAsync(next);
    }

    [RelayCommand]
    async Task MarkFalse()
    {
        var page = Shell.Current?.CurrentPage;
        if (Incident is null || page is null) return;
        bool ok = await page.DisplayAlert("Yolg'on signal",
            "Bu chaqiriqni yolg'on signal deb belgilaysizmi?", "Ha", "Yo'q");
        if (ok) await UpdateStatusAsync("False");
    }

    async Task UpdateStatusAsync(string status)
    {
        if (Incident is null) return;
        IsBusy = true;
        try
        {
            var ok = await api.PatchAsync($"/dispatch/incidents/{Incident.Id}/status",
                new { status, officer = auth.Current.Name });
            if (ok)
            {
                Incident.DispatchStatus = status;
                RefreshAction();
                await Shell.Current!.GoToAsync(".."); // dashboardga qaytadi (ro'yxat yangilanadi)
            }
            else
            {
                var page = Shell.Current?.CurrentPage;
                if (page is not null) await page.DisplayAlert("Xato", "Holatni yangilab bo'lmadi.", "OK");
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    async Task Back() => await Shell.Current!.GoToAsync("..");
}
