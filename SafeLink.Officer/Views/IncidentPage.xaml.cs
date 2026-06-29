using SafeLink.Officer.ViewModels;

namespace SafeLink.Officer.Views;

public partial class IncidentPage : ContentPage
{
	public IncidentPage(IncidentViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
