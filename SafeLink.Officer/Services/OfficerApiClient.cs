using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SafeLink.Officer.Services;

/// <summary>Dispatch API bilan ishlash. Haqiqiy telefon (USB): `adb reverse tcp:5000 tcp:5000`.</summary>
public class OfficerApiClient
{
    private readonly HttpClient _http;
    private const string BaseUrl = "http://127.0.0.1:5000";

    public OfficerApiClient() => _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };

    public void SetToken(string token) =>
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public async Task<T?> GetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<T>() : default;
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var res = await _http.PostAsJsonAsync(url, body);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<T>() : default;
    }

    public async Task<bool> PatchAsync(string url, object body)
    {
        var res = await _http.PatchAsJsonAsync(url, body);
        return res.IsSuccessStatusCode;
    }
}
