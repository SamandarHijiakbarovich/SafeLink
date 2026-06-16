using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class OrderPage : ContentPage
{
    public OrderPage(OrderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
