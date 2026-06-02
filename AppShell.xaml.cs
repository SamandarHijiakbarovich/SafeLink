using SafeLink.Views;

namespace SafeLink;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ─── Ro'yxatdan o'tmagan sahifalar (modal/push navigation) ───
        // Tab bar da ko'rinmaydi, lekin GoToAsync() bilan ochiladi.

        // SOS hold ekrani
        Routing.RegisterRoute("soshold", typeof(SosHoldPage));

        // Alert yuborildi ekrani
        Routing.RegisterRoute("alertsent", typeof(AlertSentPage));

        // Brelok ulash ekrani
        Routing.RegisterRoute("pairing", typeof(PairingPage));
    }
}
