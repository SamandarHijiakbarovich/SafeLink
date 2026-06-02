using SafeLink.Services.Interfaces;

namespace SafeLink.Services;

/// <summary>
/// GPS orqali joylashuvni aniqlash xizmati.
///
/// MAUI da geolokatsiya uchun:
/// Android: AndroidManifest.xml da ruxsat so'rash kerak
/// iOS: Info.plist da NSLocationWhenInUseUsageDescription kerak
/// </summary>
public class GeolocationService : IGeolocationService
{
    public async Task<(double Latitude, double Longitude, string? Address)> GetCurrentLocationAsync()
    {
        // Avval ruxsat so'raymiz
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            return (41.2995, 69.2401, "Toshkent, O'zbekiston"); // Default: Toshkent

        var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        var location = await Geolocation.Default.GetLocationAsync(request);

        if (location == null)
            return (41.2995, 69.2401, null);

        var address = await GetAddressAsync(location.Latitude, location.Longitude);
        return (location.Latitude, location.Longitude, address);
    }

    public async Task<string?> GetAddressAsync(double latitude, double longitude)
    {
        try
        {
            // MAUI ning o'rnatilgan Geocoding xizmati
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark == null) return null;

            // Manzilni formatlash: "Ko'cha nomi, Shahar"
            return $"{placemark.Thoroughfare ?? placemark.SubLocality}, {placemark.Locality}"
                   .Trim(',', ' ');
        }
        catch
        {
            return null;
        }
    }
}
