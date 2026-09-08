using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SafeLink.Services;

// Barcha API so'rovlar shu orqali o'tadi
public class SafeApiClient
{
    private readonly HttpClient _http;
    // Haqiqiy telefon (USB): `adb reverse tcp:5000 tcp:5000` ishga tushiring —
    // telefonning 127.0.0.1:5000 si kompyuterdagi backendga yo'naltiriladi.
    // Emulator uchun esa: "http://10.0.2.2:5000"
    private const string BaseUrl = "http://127.0.0.1:5000";

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

    public async Task<bool> PutAsync(string url, object body)
    {
        var res = await _http.PutAsJsonAsync(url, body);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string url)
    {
        var res = await _http.DeleteAsync(url);
        return res.IsSuccessStatusCode;
    }
}
