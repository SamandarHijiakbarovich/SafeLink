using SafeLink.ViewModels;

namespace SafeLink.Views;

/// <summary>
/// SosHoldPage code-behind.
///
/// Sahifa ochilganda avtomatik countdown boshlanadi.
/// Animatsiyalar: blink dot, progress ring.
/// </summary>
public partial class SosHoldPage : ContentPage
{
    private bool _blinkRunning;

    public SosHoldPage(SosHoldViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Blink animatsiyasini boshlash
        _blinkRunning = true;
        StartBlinkAnimation();

        // Countdown avtomatik boshlanadi
        var vm = (SosHoldViewModel)BindingContext;
        await vm.StartCountdownCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _blinkRunning = false;
    }

    /// <summary>
    /// "FAVQULODDA REJIM" yonidagi nuqtaning miltillashi.
    /// 0.8 soniyada bir marta yashab o'chadi.
    /// </summary>
    private async void StartBlinkAnimation()
    {
        while (_blinkRunning)
        {
            await BlinkDot.FadeTo(0.3, 400);
            await BlinkDot.FadeTo(1.0, 400);
        }
    }
}
