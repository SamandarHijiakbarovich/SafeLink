using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Services;
using SafeLink.Services.Interfaces;
using System.Collections.ObjectModel;

namespace SafeLink.ViewModels;

public partial class PairingViewModel(IBluetoothService ble) : ObservableObject
{
    public ObservableCollection<BleDevice> Devices { get; } = [];

    [ObservableProperty] bool isScanning;
    [ObservableProperty] bool isConnecting;
    [ObservableProperty] bool isConnected;
    [ObservableProperty] string connectedName = "";
    [ObservableProperty] string statusText = "Qurilmani topish uchun Scan bosing";
    [ObservableProperty] BleDevice? selectedDevice;

    CancellationTokenSource? _cts;

    [RelayCommand]
    async Task StartScan()
    {
        var allowed = await ble.RequestPermissionsAsync();
        if (!allowed)
        {
            StatusText = "Bluetooth ruxsati berilmadi";
            return;
        }

        Devices.Clear();
        IsScanning = true;
        StatusText = "Qurilmalar izlanmoqda…";

        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));

        try
        {
            await ble.StartScanAsync(device =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Devices.All(d => d.MacAddress != device.MacAddress))
                        Devices.Add(device);
                });
            }, _cts.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsScanning = false;
            StatusText = Devices.Count > 0
                ? $"{Devices.Count} ta qurilma topildi"
                : "Qurilma topilmadi. Brelokni 5 soniya bosib turing.";
        }
    }

    [RelayCommand]
    void StopScan()
    {
        _cts?.Cancel();
        IsScanning = false;
    }

    [RelayCommand]
    async Task Connect(BleDevice device)
    {
        SelectedDevice = device;
        IsConnecting = true;
        StatusText = $"{device.Name} ga ulanmoqda…";

        var ok = await ble.ConnectAsync(device.MacAddress);

        IsConnecting = false;
        if (ok)
        {
            IsConnected = true;
            ConnectedName = device.Name;
            StatusText = "Muvaffaqiyatli ulandi!";
            await Task.Delay(1200);
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            StatusText = "Ulanishda xato. Qayta urinib ko'ring.";
            SelectedDevice = null;
        }
    }

    [RelayCommand]
    async Task Skip() => await Shell.Current.GoToAsync("..");
}
