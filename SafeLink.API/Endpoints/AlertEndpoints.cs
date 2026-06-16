using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SafeLink.API.Data;
using SafeLink.API.Models;
using SafeLink.API.Services;

namespace SafeLink.API.Endpoints;

public static class AlertEndpoints
{
    public static void MapAlerts(this WebApplication app)
    {
        var g = app.MapGroup("/alerts").WithTags("Alerts").RequireAuthorization();

        // SOS yuborish
        g.MapPost("/send", async (SendAlertRequest req, ClaimsPrincipal principal, AppDbContext db, SmsService sms) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await db.Users
                .Include(u => u.TrustedContacts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) return Results.Unauthorized();

            var alert = new Alert
            {
                UserId = userId,
                Latitude = req.Latitude,
                Longitude = req.Longitude,
                Address = req.Address,
                Status = AlertStatus.Active,
                PoliceEtaMinutes = 4,
            };
            db.Alerts.Add(alert);
            await db.SaveChangesAsync();

            // Ishonchli kontaktlarga SMS yuborish
            foreach (var contact in user.TrustedContacts)
                await sms.SendEmergencyAlertAsync(contact.PhoneNumber, user.FullName, req.Address ?? "noma'lum");

            return Results.Ok(new { alertId = alert.Id, policeEta = alert.PoliceEtaMinutes });
        });

        // Alertni bekor qilish (yolg'on signal)
        g.MapPatch("/cancel/{id:int}", async (int id, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var alert = await db.Alerts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (alert is null) return Results.NotFound();
            alert.Status = AlertStatus.FalseAlarm;
            alert.ResolvedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        // Tarix
        g.MapGet("/history", async (ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var alerts = await db.Alerts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.SentAt)
                .Select(a => new { a.Id, a.Address, a.Status, a.SentAt, a.PoliceEtaMinutes })
                .ToListAsync();

            return Results.Ok(alerts);
        });
    }

    record SendAlertRequest(double Latitude, double Longitude, string? Address);
}
