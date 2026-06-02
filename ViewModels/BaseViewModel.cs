using CommunityToolkit.Mvvm.ComponentModel;

namespace SafeLink.ViewModels;

/// <summary>
/// Barcha ViewModel lar shu sinfdan meros oladi.
///
/// CommunityToolkit.Mvvm ishlatamiz, chunki:
/// - [ObservableProperty] — avtomatik INotifyPropertyChanged
/// - [RelayCommand] — UI dan Command bog'lash
/// - Kodni ancha qisqartiradi
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    // [ObservableProperty] atributi avtomatik:
    // 1. private field yaratadi (isBusy)
    // 2. public property yaratadi (IsBusy)
    // 3. PropertyChanged event chaqiradi

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    // IsBusy ning teskarisi — UI binding uchun qulay
    public bool IsNotBusy => !IsBusy;
}
