using SafeLink.Services;

namespace SafeLink;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly AuthService _auth;

    public App(AppShell shell, AuthService auth)
    {
        InitializeComponent();
        _shell = shell;
        _auth = auth;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_shell);

    protected override async void OnStart()
    {
        base.OnStart();
        // Saqlangan token bo'lsa API ga ulash
        await _auth.RestoreSessionAsync();
    }
}
