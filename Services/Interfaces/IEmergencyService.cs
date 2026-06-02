using SafeLink.Models;

namespace SafeLink.Services.Interfaces;

/// <summary>
/// Favqulodda vaziyat xizmati — SOS signalni boshqaradi.
/// </summary>
public interface IEmergencyService
{
    /// <summary>
    /// SOS signal yuborish: joylashuv, audio, kontaktlar, 102.
    /// progress: 0.0 dan 1.0 gacha (3 soniya davomida)
    /// </summary>
    Task<AlertEvent> SendAlertAsync(IProgress<double> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Yolg'on signal — aktiv alertni bekor qilish.
    /// </summary>
    Task CancelAlertAsync(AlertEvent alert);

    /// <summary>
    /// Operator bilan bog'lanish (102).
    /// </summary>
    Task CallOperatorAsync();
}
