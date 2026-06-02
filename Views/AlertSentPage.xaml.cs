using SafeLink.ViewModels;

namespace SafeLink.Views;

public partial class AlertSentPage : ContentPage
{
    private bool _blinkRunning;

    public AlertSentPage(AlertSentViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _blinkRunning = true;
        StartActiveDotBlink();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _blinkRunning = false;
    }

    /// <summary>
    /// "Faol" badge dagi yashil nuqta miltillashi.
    /// </summary>
    private async void StartActiveDotBlink()
    {
        while (_blinkRunning)
        {
            await ActiveDot.FadeTo(0.3, 600);
            await ActiveDot.FadeTo(1.0, 600);
        }
    }
}
