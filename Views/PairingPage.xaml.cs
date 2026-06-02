namespace SafeLink.Views;

public partial class PairingPage : ContentPage
{
    public PairingPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await StartRadarAnimation();
    }

    /// <summary>
    /// Radar halqalar animatsiyasi — brelok qidirilmoqda ko'rinishi.
    /// </summary>
    private async Task StartRadarAnimation()
    {
        while (true)
        {
            await Task.WhenAll(
                Radar1.ScaleTo(1.4, 2000, Easing.CubicOut),
                Radar1.FadeTo(0, 2000, Easing.CubicOut)
            );
            Radar1.Scale = 0.6;
            Radar1.Opacity = 0.5;

            await Task.WhenAll(
                Radar2.ScaleTo(1.4, 2000, Easing.CubicOut),
                Radar2.FadeTo(0, 2000, Easing.CubicOut)
            );
            Radar2.Scale = 0.6;
            Radar2.Opacity = 0.5;
        }
    }
}
