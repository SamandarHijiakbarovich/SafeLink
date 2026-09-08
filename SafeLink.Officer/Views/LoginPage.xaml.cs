using SafeLink.Officer.ViewModels;

namespace SafeLink.Officer.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
