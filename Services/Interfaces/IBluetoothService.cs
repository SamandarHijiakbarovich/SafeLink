namespace SafeLink.Services.Interfaces;

public interface IBluetoothService
{
    bool IsAvailable { get; }
    bool IsScanning { get; }
    Task<bool> RequestPermissionsAsync();
    Task StartScanAsync(Action<BleDevice> onDeviceFound, CancellationToken ct);
    Task StopScanAsync();
    Task<bool> ConnectAsync(string macAddress);
    Task DisconnectAsync();
    event EventHandler<BleConnectionChangedArgs> ConnectionChanged;
}

public record BleDevice(string Name, string MacAddress, int Rssi, bool IsKnownSafeLink);

public class BleConnectionChangedArgs(bool connected, string? deviceName) : EventArgs
{
    public bool IsConnected { get; } = connected;
    public string? DeviceName { get; } = deviceName;
}
