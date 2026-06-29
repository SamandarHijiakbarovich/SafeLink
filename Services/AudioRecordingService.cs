using Plugin.Maui.Audio;

namespace SafeLink.Services;

/// <summary>
/// SOS paytida atrofdagi ovozni mikrofondan yozib oladi (dalil sifatida).
/// Mikrofon ruxsati so'raladi; yozuv to'xtatilganda fayl AppData'ga saqlanadi.
/// </summary>
public class AudioRecordingService(IAudioManager audioManager)
{
    private IAudioRecorder? _recorder;

    public bool IsRecording => _recorder?.IsRecording ?? false;

    public async Task<bool> StartAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted) return false;

            _recorder = audioManager.CreateRecorder();
            await _recorder.StartAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Yozuvni to'xtatadi va saqlangan fayl yo'lini qaytaradi (yoki null).</summary>
    public async Task<string?> StopAsync()
    {
        try
        {
            if (_recorder is null || !_recorder.IsRecording) return null;

            var source = await _recorder.StopAsync();
            var path = Path.Combine(FileSystem.AppDataDirectory, $"sos_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

            await using var stream = source.GetAudioStream();
            await using var file = File.Create(path);
            await stream.CopyToAsync(file);
            return path;
        }
        catch
        {
            return null;
        }
        finally
        {
            _recorder = null;
        }
    }
}
