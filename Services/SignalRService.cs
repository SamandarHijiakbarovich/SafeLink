using Microsoft.AspNetCore.SignalR.Client;

namespace SafeLink.Services;

/// <summary>
/// Fuqaro ilovasi uchun real-time ulanish.
/// SOS yuborgandan so'ng politsiya statusini darhol ko'rsatish uchun.
/// </summary>
public class SignalRService
{
    private HubConnection? _hub;
    private const string HubUrl = "http://10.0.2.2:5000/hubs/alerts";

    public event Action<string, string?>? StatusUpdated;

    public async Task ConnectAsync(string token, int userId)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(HubUrl, opt => opt.AccessTokenProvider = () => Task.FromResult<string?>(token))
            .WithAutomaticReconnect()
            .Build();

        _hub.On<StatusPayload>("StatusUpdate", payload =>
            MainThread.BeginInvokeOnMainThread(() =>
                StatusUpdated?.Invoke(payload.DispatchStatus, payload.AssignedOfficer)));

        await _hub.StartAsync();
        await _hub.SendAsync("JoinUser", userId.ToString());
    }

    public async Task DisconnectAsync()
    {
        if (_hub is not null)
            await _hub.StopAsync();
    }

    record StatusPayload(int Id, string DispatchStatus, string? AssignedOfficer);
}
