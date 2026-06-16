using SafeLink.ViewModels.Onboarding;

namespace SafeLink.Views.Onboarding;

public partial class PhonePage : ContentPage
{
    private PhoneViewModel _vm = null!;

    public PhonePage(PhoneViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    void OnCtaClicked(object sender, EventArgs e)
    {
        if (_vm.OtpSent)
            _vm.VerifyOtpCommand.Execute(null);
        else
            _vm.SendOtpCommand.Execute(null);
    }
}
