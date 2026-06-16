using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SafeLink.Services;

// Barcha API so'rovlar shu orqali o'tadi
public class SafeApiClient
{
    private readonly HttpClient _http;
    private const string BaseUrl = "http://10.0.2.2:5000"; // Android emulator → localhost

    public SafeApiClient()
    {
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public void SetToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return default;
        return await res.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var res = await _http.PostAsJsonAsync(url, body);
        if (!res.IsSuccessStatusCode) return default;
        return await res.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> PatchAsync(string url)
    {
        var res = await _http.PatchAsync(url, null);
        return res.IsSuccessStatusCode;
    }
}
