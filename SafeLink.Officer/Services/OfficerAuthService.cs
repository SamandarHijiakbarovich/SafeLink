using SafeLink.Officer.Models;

namespace SafeLink.Officer.Services;

public class OfficerAuthService(OfficerApiClient api)
{
    private const string TokenKey = "officer_token";
    private const string NameKey = "officer_name";
    private const string UnitKey = "officer_unit";
    private const string IdKey = "officer_id";

    public OfficerInfo Current { get; private set; } = new();

    public async Task<bool> LoginAsync(string serviceId, string password)
    {
        var res = await api.PostAsync<LoginResponse>("/dispatch/login",
            new { serviceId, password });
        if (res is null) return false;

        Current = new OfficerInfo { ServiceId = res.Officer.ServiceId, Name = res.Officer.Name, Unit = res.Officer.Unit, Token = res.Token };
        api.SetToken(res.Token);
        await SecureStorage.SetAsync(TokenKey, res.Token);
        await SecureStorage.SetAsync(NameKey, Current.Name);
        await SecureStorage.SetAsync(UnitKey, Current.Unit);
        await SecureStorage.SetAsync(IdKey, Current.ServiceId);
        return true;
    }

    public async Task<bool> RestoreSessionAsync()
    {
        var token = await SecureStorage.GetAsync(TokenKey);
        if (string.IsNullOrEmpty(token)) return false;
        api.SetToken(token);
        Current = new OfficerInfo
        {
            ServiceId = await SecureStorage.GetAsync(IdKey) ?? "",
            Name = await SecureStorage.GetAsync(NameKey) ?? "Xodim",
            Unit = await SecureStorage.GetAsync(UnitKey) ?? "",
            Token = token,
        };
        return true;
    }

    public void Logout()
    {
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(NameKey);
        SecureStorage.Remove(UnitKey);
        SecureStorage.Remove(IdKey);
    }

    record LoginResponse(string Token, OfficerDto Officer);
    record OfficerDto(string ServiceId, string Name, string Unit);
}
