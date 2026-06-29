using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Officer.Models;
using SafeLink.Officer.Services;

namespace SafeLink.Officer.ViewModels;

public partial class DashboardViewModel(OfficerApiClient api, OfficerAuthService auth) : ObservableObject
{
    [ObservableProperty] string officerName = "";
    [ObservableProperty] string officerUnit = "";
    [ObservableProperty] string officerInitials = "JS";
    [ObservableProperty] bool isLoading;
    [ObservableProperty] bool isEmpty;
    [ObservableProperty] string clock = "";

    // Statistika
    [ObservableProperty] string todayCalls = "0";
    [ObservableProperty] string avgResponse = "—";
    [ObservableProperty] string activePatrols = "0";
    [ObservableProperty] string completed = "0";

    public ObservableCollection<Incident> Incidents { get; } = [];

    public async Task LoadAsync()
    {
        OfficerName = auth.Current.Name;
        OfficerUnit = auth.Current.Unit;
        OfficerInitials = auth.Current.Initials;
        Clock = DateTime.Now.ToString("dd.MM.yyyy · HH:mm");

        IsLoading = true;
        try
        {
            var stats = await api.GetAsync<StatsDto>("/dispatch/stats");
            if (stats is not null)
            {
                TodayCalls = stats.TodayCalls.ToString();
                AvgResponse = stats.AvgResponse;
                ActivePatrols = stats.ActivePatrols.ToString();
                Completed = stats.Completed.ToString();
            }

            var items = await api.GetAsync<List<Incident>>("/dispatch/incidents");
            Incidents.Clear();
            foreach (var i in items ?? [])
                Incidents.Add(i);
        }
        catch { /* tarmoq xatosi — bo'sh holat */ }
        finally
        {
            IsLoading = false;
            IsEmpty = Incidents.Count == 0;
        }
    }

    [RelayCommand]
    async Task Refresh() => await LoadAsync();

    [RelayCommand]
    async Task OpenIncident(Incident incident)
    {
        if (incident is null) return;
        await Shell.Current.GoToAsync("incident",
            new Dictionary<string, object> { ["Incident"] = incident });
    }

    [RelayCommand]
    async Task Logout()
    {
        var page = Shell.Current?.CurrentPage;
        bool ok = page is not null && await page.DisplayAlert("Chiqish", "Tizimdan chiqasizmi?", "Ha", "Yo'q");
        if (!ok) return;
        auth.Logout();
        await Shell.Current!.GoToAsync("//login");
    }

    record StatsDto(int TodayCalls, string AvgResponse, int ActivePatrols, int Completed);
}
