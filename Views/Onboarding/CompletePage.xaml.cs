using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class CompletePage : ContentPage
{
    public CompletePage(CompleteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
