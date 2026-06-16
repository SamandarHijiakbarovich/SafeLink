using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class IdentityPage : ContentPage
{
    public IdentityPage(IdentityViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
