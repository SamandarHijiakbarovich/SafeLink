namespace SafeLink.Services.Interfaces;

/// <summary>
/// GPS joylashuv xizmati.
/// </summary>
public interface IGeolocationService
{
    /// <summary>
    /// Hozirgi joylashuvni olish.
    /// </summary>
    Task<(double Latitude, double Longitude, string? Address)> GetCurrentLocationAsync();

    /// <summary>
    /// Koordinatalardan manzil olish (reverse geocoding).
    /// </summary>
    Task<string?> GetAddressAsync(double latitude, double longitude);
}
