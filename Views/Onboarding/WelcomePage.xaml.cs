using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class WelcomePage : ContentPage
{
    public WelcomePage(WelcomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
