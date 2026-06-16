using SafeLink.Services;
using SafeLink.Views;

namespace SafeLink;

public partial class AppShell : Shell
{
    private readonly AuthService _auth;

    public AppShell(AuthService auth)
    {
        InitializeComponent();
        _auth = auth;

        Routing.RegisterRoute("soshold", typeof(SosHoldPage));
        Routing.RegisterRoute("alertsent", typeof(AlertSentPage));
        Routing.RegisterRoute("main/pairing", typeof(PairingPage));
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        // Ilova birinchi marta ochilganda token tekshirish
        if (args.Target.Location.OriginalString == "//")
        {
            args.Cancel();
            var hasSession = await _auth.RestoreSessionAsync();
            await GoToAsync(hasSession ? "//main" : "//welcome");
        }
    }
}
