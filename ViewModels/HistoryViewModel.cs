using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SafeLink.Models;
using SafeLink.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SafeLink.ViewModels;

public partial class HistoryViewModel(SafeApiClient api) : ObservableObject
{
    public ObservableCollection<AlertEvent> Alerts { get; } = [];

    [ObservableProperty] bool isLoading;
    [ObservableProperty] bool isEmpty;
    [ObservableProperty] bool hasError;

    [RelayCommand]
    async Task LoadHistory()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var items = await api.GetAsync<List<AlertDto>>("/alerts/history");
            Alerts.Clear();

            if (items is { Count: > 0 })
            {
                foreach (var d in items)
                    Alerts.Add(new AlertEvent
                    {
                        Id = d.Id,
                        Address = d.Address ?? "Manzil noma'lum",
                        Status = d.Status switch
                        {
                            "FalseAlarm" => AlertStatus.FalseAlarm,
                            "Resolved"   => AlertStatus.Resolved,
                            _            => AlertStatus.Active,
                        },
                        SentAt = d.SentAt,
                        PoliceEtaMinutes = d.PoliceEtaMinutes,
                    });
            }
        }
        catch
        {
            // Oflayn: namuna ma'lumotlar ko'rsatish
            LoadDemoData();
        }
        finally
        {
            IsLoading = false;
            IsEmpty = Alerts.Count == 0;
        }
    }

    [RelayCommand]
    async Task OpenDetail(AlertEvent alert)
    {
        // TODO: detail sahifaga o'tish
        await Task.CompletedTask;
    }

    void LoadDemoData()
    {
        Alerts.Clear();
        Alerts.Add(new AlertEvent { Id = 1, SentAt = DateTime.Now.AddDays(-1).AddHours(-3), Address = "Mirzo Ulug'bek t., Buyuk Ipak yo'li 12", Status = AlertStatus.Resolved, PoliceEtaMinutes = 4 });
        Alerts.Add(new AlertEvent { Id = 2, SentAt = DateTime.Now.AddDays(-5).AddHours(-7), Address = "Yunusobod t., Amir Temur ko'ch. 45", Status = AlertStatus.FalseAlarm, PoliceEtaMinutes = 6 });
        Alerts.Add(new AlertEvent { Id = 3, SentAt = DateTime.Now.AddDays(-12).AddHours(-2), Address = "Chilonzor t., Qoratosh ko'ch. 8", Status = AlertStatus.Resolved, PoliceEtaMinutes = 5 });
    }

    record AlertDto(
        int Id,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("sentAt")] DateTime SentAt,
        [property: JsonPropertyName("policeEtaMinutes")] int? PoliceEtaMinutes
    );
}
