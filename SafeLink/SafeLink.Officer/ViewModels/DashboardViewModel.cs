using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Officer.Models;
using SafeLink.Officer.Services;

namespace SafeLink.Officer.ViewModels;

public partial class DashboardViewModel(OfficerApiClient api, OfficerAuthService auth, SignalRService signalR) : ObservableObject
{
    [ObservableProperty] string officerName = "";
    [ObservableProperty] string officerUnit = "";
    [ObservableProperty] string officerInitials = "JS";
    [ObservableProperty] bool isLoading;
    [ObservableProperty] bool isEmpty;
    [ObservableProperty] string clock = "";
    [ObservableProperty] bool hasNewAlert;
    [ObservableProperty] string newAlertText = "";

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

        signalR.NewAlertReceived -= OnNewAlert;
        signalR.NewAlertReceived += OnNewAlert;
        signalR.AlertStatusChanged -= OnStatusChanged;
        signalR.AlertStatusChanged += OnStatusChanged;
        _ = ConnectSignalR();

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
        catch { }
        finally
        {
            IsLoading = false;
            IsEmpty = Incidents.Count == 0;
        }
    }

    async Task ConnectSignalR()
    {
        try { await signalR.ConnectAsync(auth.Current.Token); }
        catch { }
    }

    void OnNewAlert(NewAlertPayload p)
    {
        var incident = new Incident
        {
            Id = p.Id,
            CitizenName = p.CitizenName,
            Address = p.Address,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            DispatchStatus = "New",
            SentAt = p.SentAt,
            PoliceEtaMinutes = p.PoliceEtaMinutes,
        };
        Incidents.Insert(0, incident);
        IsEmpty = false;

        if (int.TryParse(TodayCalls, out var n)) TodayCalls = (n + 1).ToString();

        NewAlertText = $"\U0001f6a8 Yangi SOS — {p.CitizenName}";
        HasNewAlert = true;
        _ = HideBannerAfterDelay();
    }

    void OnStatusChanged(StatusChangedPayload p)
    {
        var inc = Incidents.FirstOrDefault(i => i.Id == p.Id);
        if (inc is null) return;
        inc.DispatchStatus = p.DispatchStatus;
        inc.AssignedOfficer = p.AssignedOfficer;
        var idx = Incidents.IndexOf(inc);
        Incidents[idx] = inc;
    }

    async Task HideBannerAfterDelay()
    {
        await Task.Delay(5000);
        HasNewAlert = false;
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
        await signalR.DisconnectAsync();
        auth.Logout();
        await Shell.Current!.GoToAsync("//login");
    }

    record StatsDto(int TodayCalls, string AvgResponse, int ActivePatrols, int Completed);
}
