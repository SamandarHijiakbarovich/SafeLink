using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SafeLink.ViewModels.Onboarding;

public partial class OrderViewModel : ObservableObject
{
    [ObservableProperty] string orderNumber = "";
    [ObservableProperty] bool hasFile;

    [RelayCommand]
    async Task PickFile()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Himoya orderi (PDF)",
            FileTypes = FilePickerFileType.Pdf,
        });
        if (result is null) return;
        HasFile = true;
        OrderNumber = "2509-114"; // TODO: OCR orqali avtomatik aniqlash
    }

    [RelayCommand]
    async Task Continue() => await Shell.Current.GoToAsync("//onboarding/contacts");

    [RelayCommand]
    async Task Skip() => await Shell.Current.GoToAsync("//onboarding/contacts");
}
