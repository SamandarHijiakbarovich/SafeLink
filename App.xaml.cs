namespace SafeLink;

public partial class App : Application
{
    private readonly AppShell _shell;

    // AppShell DI orqali keladi
    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_shell);
}
