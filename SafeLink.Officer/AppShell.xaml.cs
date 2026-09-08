using SafeLink.Officer.Services;
using SafeLink.Officer.Views;

namespace SafeLink.Officer;

public partial class AppShell : Shell
{
	private readonly OfficerAuthService _auth;

	public AppShell(OfficerAuthService auth)
	{
		InitializeComponent();
		_auth = auth;
		Routing.RegisterRoute("incident", typeof(IncidentPage));
	}

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		base.OnNavigating(args);
		if (args.Target.Location.OriginalString == "//")
		{
			args.Cancel();
			var hasSession = await _auth.RestoreSessionAsync();
			await GoToAsync(hasSession ? "//dashboard" : "//login");
		}
	}
}
