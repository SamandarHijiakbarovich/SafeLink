namespace SafeLink.API.Services;

// Eskiz.uz yoki PlayMobile SMS gateway
public class SmsService(IConfiguration config, ILogger<SmsService> logger)
{
    private readonly HttpClient _http = new();

    public async Task<bool> SendOtpAsync(string phone, string code)
    {
        var message = $"SafeLink: Tasdiqlash kodingiz {code}. 5 daqiqa ichida foydalaning.";
        return await SendAsync(phone, message);
    }

    public async Task<bool> SendEmergencyAlertAsync(string phone, string userName, string address)
    {
        var message = $"XAVF! {userName} favqulodda yordam so'radi. Joylashuv: {address}. SafeLink tizimi.";
        return await SendAsync(phone, message);
    }

    private async Task<bool> SendAsync(string phone, string message)
    {
        try
        {
            // Eskiz.uz API
            var login = config["Sms:Login"];
            var password = config["Sms:Password"];
            var from = config["Sms:From"] ?? "SafeLink";

            var payload = new
            {
                messages = new[]
                {
                    new { recipient = phone, message_id = Guid.NewGuid().ToString("N")[..8], sms = new { originator = from, content = new { text = message } } }
                }
            };

            // TODO: real Eskiz.uz endpoint
            logger.LogInformation("SMS -> {Phone}: {Message}", phone, message);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMS yuborishda xato: {Phone}", phone);
            return false;
        }
    }
}
