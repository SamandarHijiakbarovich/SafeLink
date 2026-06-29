using SafeLink.Officer.Services;

namespace SafeLink.Officer;

public partial class App : Application
{
	private readonly AppShell _shell;
	private readonly OfficerAuthService _auth;

	public App(AppShell shell, OfficerAuthService auth)
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
		await _auth.RestoreSessionAsync();
	}
}
