using Microsoft.AspNetCore.SignalR;

namespace SafeLink.API.Hubs;

/// <summary>
/// Real-time SOS xabarlari: fuqaro SOS bosadi → barcha ulanган officer larga darhol yuboriladi.
/// </summary>
public class AlertHub : Hub
{
    // Officer ilova ulanadi: groups["officers"] ga qo'shiladi
    public async Task JoinOfficers() =>
        await Groups.AddToGroupAsync(Context.ConnectionId, "officers");

    // Fuqaro bekor qilish so'rasa
    public async Task JoinUser(string userId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
}
