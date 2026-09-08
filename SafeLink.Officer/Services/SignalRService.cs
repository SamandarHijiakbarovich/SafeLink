using Microsoft.AspNetCore.SignalR.Client;

namespace SafeLink.Officer.Services;

/// <summary>
/// Real-time ulanish: yangi SOS yoki holat yangilanishi darhol ko'rinadi.
/// </summary>
public class SignalRService
{
    private HubConnection? _hub;
    private const string HubUrl = "http://127.0.0.1:5000/hubs/alerts";

    public event Action<NewAlertPayload>? NewAlertReceived;
    public event Action<StatusChangedPayload>? AlertStatusChanged;

    public async Task ConnectAsync(string token)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(HubUrl, opt => opt.AccessTokenProvider = () => Task.FromResult<string?>(token))
            .WithAutomaticReconnect()
            .Build();

        _hub.On<NewAlertPayload>("NewAlert", payload =>
            MainThread.BeginInvokeOnMainThread(() => NewAlertReceived?.Invoke(payload)));

        _hub.On<StatusChangedPayload>("AlertStatusChanged", payload =>
            MainThread.BeginInvokeOnMainThread(() => AlertStatusChanged?.Invoke(payload)));

        await _hub.StartAsync();
        await _hub.SendAsync("JoinOfficers");
    }

    public async Task DisconnectAsync()
    {
        if (_hub is not null)
            await _hub.StopAsync();
    }
}

public record NewAlertPayload(int Id, string CitizenName, string? Address, double Latitude, double Longitude, int? PoliceEtaMinutes, DateTime SentAt, string DispatchStatus);
public record StatusChangedPayload(int Id, string DispatchStatus, string? AssignedOfficer);
