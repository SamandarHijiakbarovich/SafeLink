using SafeLink.ViewModels;

namespace SafeLink.Views;

public partial class PairingPage : ContentPage
{
    public PairingPage(PairingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
