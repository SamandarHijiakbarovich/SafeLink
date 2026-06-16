using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SafeLink.Services;
using SafeLink.Services.Interfaces;
using SafeLink.ViewModels;
using SafeLink.Views;

namespace SafeLink;

/// <summary>
/// Ilovaning kirish nuqtasi — barcha xizmatlar va sahifalar shu yerda ro'yxatdan o'tadi.
///
/// Dependency Injection (DI) nima?
/// ─────────────────────────────────
/// Har bir sinf o'z kerakli ob'ektlarini o'zi yasab olmaydi.
/// Buning o'rniga, MauiProgram barcha kerakli narsalarni yaratib,
/// konstruktorda uzatadi.
///
/// Misol:
///   HomeViewModel → IEmergencyService kerak
///   IEmergencyService → IGeolocationService kerak
///   DI bularni avtomatik yaratib beradi.
///
/// AddSingleton → bir marta yaratiladi, doim shu nusxa ishlatiladi
/// AddTransient → har safar yangi nusxa yaratiladi
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // CommunityToolkit.Maui ni faollashtirish
            // (BoolToObjectConverter va boshqa helper lar uchun)
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ─── Xizmatlar (Services) ───────────────────────────────
        builder.Services.AddSingleton<SafeApiClient>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
        builder.Services.AddSingleton<IEmergencyService, EmergencyService>();

        // ─── ViewModels ─────────────────────────────────────────
        // AddTransient: har bir sahifa ochilganda yangi ViewModel
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SosHoldViewModel>();
        builder.Services.AddTransient<AlertSentViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // ─── Views (Sahifalar) ──────────────────────────────────
        // Sahifa → ViewModel ni DI uzatadi (konstruktor orqali)
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SosHoldPage>();
        builder.Services.AddTransient<AlertSentPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<PairingPage>();

        // ─── AppShell ───────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
