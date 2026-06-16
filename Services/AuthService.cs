namespace SafeLink.Services;

public class AuthService(SafeApiClient api)
{
    private const string TokenKey = "jwt_token";
    private const string UserIdKey = "user_id";

    public async Task<bool> SendOtpAsync(string phone)
    {
        var res = await api.PostAsync<object>("/auth/send-otp", new { phone });
        return res is not null;
    }

    public async Task<(bool success, bool isNewUser)> VerifyOtpAsync(string phone, string code)
    {
        var res = await api.PostAsync<VerifyResponse>("/auth/verify-otp", new { phone, code });
        if (res is null) return (false, false);

        await SecureStorage.SetAsync(TokenKey, res.Token);
        await SecureStorage.SetAsync(UserIdKey, res.UserId.ToString());
        api.SetToken(res.Token);
        return (true, res.IsNewUser);
    }

    public async Task<bool> RestoreSessionAsync()
    {
        var token = await SecureStorage.GetAsync(TokenKey);
        if (string.IsNullOrEmpty(token)) return false;
        api.SetToken(token);
        return true;
    }

    public bool IsLoggedIn => SecureStorage.GetAsync(TokenKey).GetAwaiter().GetResult() is not null;

    public void Logout()
    {
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(UserIdKey);
    }

    record VerifyResponse(string Token, bool IsNewUser, int UserId);
}
