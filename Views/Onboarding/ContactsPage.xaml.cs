using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class ContactsPage : ContentPage
{
    public ContactsPage(ContactsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
