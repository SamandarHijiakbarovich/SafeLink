using SafeLink.ViewModels;

namespace SafeLink.Views;

/// <summary>
/// HomePage code-behind.
///
/// XAML faylda ko'rsatib bo'lmaydigan narsalar
/// (animatsiyalar) shu yerda yoziladi.
/// </summary>
public partial class HomePage : ContentPage
{
    private bool _animationRunning;
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        StartPulseAnimation();
        await _vm.LoadAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animationRunning = false;
    }

    /// <summary>
    /// SOS tugmasining puls animatsiyasi.
    ///
    /// MAUI da CSS animatsiya yo'q, shuning uchun
    /// C# da ScaleTo() va FadeTo() ishlatamiz.
    ///
    /// HaloOuter va HaloInner — XAML da e'lon qilingan
    /// Ellipse elementlari (x:Name bilan).
    /// </summary>
    private async void StartPulseAnimation()
    {
        _animationRunning = true;

        while (_animationRunning)
        {
            // Ikki halqa asinxron pulslanadi (vaqt siljishi bilan)
            var task1 = PulseElement(HaloOuter, delay: 0);
            var task2 = PulseElement(HaloInner, delay: 400);

            await Task.WhenAll(task1, task2);
            await Task.Delay(200);
        }
    }

    private async Task PulseElement(View element, int delay)
    {
        if (delay > 0)
            await Task.Delay(delay);

        // Kengayib ketadi va yo'qoladi
        await Task.WhenAll(
            element.ScaleTo(1.5, 1800, Easing.CubicOut),
            element.FadeTo(0, 1800, Easing.CubicOut)
        );

        // Qayta tiklanadi
        element.Scale = 1.0;
        element.Opacity = element == HaloOuter ? 0.15 : 0.22;
    }
}
