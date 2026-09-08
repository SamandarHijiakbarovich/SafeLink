using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using SafeLink.Services;
using SafeLink.Services.Interfaces;
using SafeLink.ViewModels;
using SafeLink.ViewModels.Onboarding;
using SafeLink.Views;
using SafeLink.Views.Onboarding;

namespace SafeLink;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ─── Xizmatlar (Services) ───────────────────────────────
        builder.Services.AddSingleton<SafeApiClient>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<AudioRecordingService>();
        builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
        builder.Services.AddSingleton<IEmergencyService, EmergencyService>();
        builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
        builder.Services.AddSingleton<SignalRService>();

        // ─── ViewModels ─────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SosHoldViewModel>();
        builder.Services.AddTransient<AlertSentViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<PairingViewModel>();

        // Onboarding ViewModels
        builder.Services.AddTransient<WelcomeViewModel>();
        builder.Services.AddTransient<PhoneViewModel>();
        builder.Services.AddTransient<IdentityViewModel>();
        builder.Services.AddTransient<OrderViewModel>();
        builder.Services.AddTransient<ContactsViewModel>();
        builder.Services.AddTransient<CompleteViewModel>();

        // ─── Views (Sahifalar) ──────────────────────────────────
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SosHoldPage>();
        builder.Services.AddTransient<AlertSentPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<PairingPage>();

        // Onboarding Views
        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<PhonePage>();
        builder.Services.AddTransient<IdentityPage>();
        builder.Services.AddTransient<OrderPage>();
        builder.Services.AddTransient<ContactsPage>();
        builder.Services.AddTransient<CompletePage>();

        // ─── AppShell ───────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
