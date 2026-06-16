using SafeLink.Services.Interfaces;

namespace SafeLink.Services;

// Real Bluetooth implementatsiyasi — Plugin.BLE (keyinroq qo'shiladi)
// Hozir: mock, prototip va emulator uchun
public class BluetoothService : IBluetoothService
{
    public bool IsAvailable => true;
    public bool IsScanning { get; private set; }

    public event EventHandler<BleConnectionChangedArgs>? ConnectionChanged;

    public async Task<bool> RequestPermissionsAsync()
    {
        var bt = await Permissions.RequestAsync<Permissions.Bluetooth>();
        return bt == PermissionStatus.Granted;
    }

    public async Task StartScanAsync(Action<BleDevice> onDeviceFound, CancellationToken ct)
    {
        IsScanning = true;

        // Mock qurilmalar — real BLE o'rniga
        await Task.Delay(1200, ct);
        if (!ct.IsCancellationRequested)
            onDeviceFound(new BleDevice("SafeLink SL-100", "AA:BB:CC:DD:EE:01", -55, true));

        await Task.Delay(1800, ct);
        if (!ct.IsCancellationRequested)
            onDeviceFound(new BleDevice("SafeLink SL-100 #2", "AA:BB:CC:DD:EE:02", -72, true));

        await Task.Delay(1000, ct);
        if (!ct.IsCancellationRequested)
            onDeviceFound(new BleDevice("BLE_Unknown_Device", "FF:EE:DD:CC:BB:01", -85, false));

        IsScanning = false;
    }

    public async Task StopScanAsync()
    {
        IsScanning = false;
        await Task.CompletedTask;
    }

    public async Task<bool> ConnectAsync(string macAddress)
    {
        // Mock ulanish: 2 soniya kutish
        await Task.Delay(2000);
        ConnectionChanged?.Invoke(this, new BleConnectionChangedArgs(true, "SafeLink SL-100"));
        return true;
    }

    public async Task DisconnectAsync()
    {
        await Task.Delay(500);
        ConnectionChanged?.Invoke(this, new BleConnectionChangedArgs(false, null));
    }
}
