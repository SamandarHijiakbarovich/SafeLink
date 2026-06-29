using Microsoft.EntityFrameworkCore;
using SafeLink.API.Data;
using SafeLink.API.Models;
using SafeLink.API.Services;

namespace SafeLink.API.Endpoints;

/// <summary>
/// IIV xodimi (Dispatch) endpointlari — favqulodda chaqiriqlarni qabul qilish va boshqarish.
/// HOZIR: login mock (Xizmat ID + parol istalgan qiymat). Keyin real xodim bazasiga ulanadi.
/// </summary>
public static class DispatchEndpoints
{
    public static void MapDispatch(this WebApplication app)
    {
        var g = app.MapGroup("/dispatch").WithTags("Dispatch");

        // 1. Xodim login (mock — istalgan ID/parol qabul qilinadi)
        g.MapPost("/login", (OfficerLoginRequest req, TokenService tokens) =>
        {
            if (string.IsNullOrWhiteSpace(req.ServiceId))
                return Results.BadRequest(new { error = "Xizmat ID kiriting" });

            // Mock xodim
            var officer = new User { Id = 90000, PhoneNumber = req.ServiceId, FullName = "Serj. J. Sodiqov" };
            return Results.Ok(new
            {
                token = tokens.Generate(officer),
                officer = new { serviceId = req.ServiceId, name = "Serj. J. Sodiqov", unit = "Patrol-04 · Mirzo Ulug'bek" },
            });
        });

        // 2. Hodisalar ro'yxati (barcha chaqiriqlar)
        g.MapGet("/incidents", async (AppDbContext db) =>
        {
            var items = await db.Alerts
                .Include(a => a.User)
                .OrderByDescending(a => a.SentAt)
                .Select(a => new
                {
                    a.Id,
                    citizenName = a.User.FullName,
                    a.Address,
                    a.Latitude,
                    a.Longitude,
                    dispatchStatus = a.DispatchStatus,
                    a.AssignedOfficer,
                    a.SentAt,
                    a.PoliceEtaMinutes,
                })
                .ToListAsync();
            return Results.Ok(items);
        }).RequireAuthorization();

        // 3. Hodisa holatini yangilash (Qabul qilish → Yo'lda → Joyda → Yakunlash)
        g.MapPatch("/incidents/{id:int}/status", async (int id, UpdateStatusRequest req, AppDbContext db) =>
        {
            var alert = await db.Alerts.FirstOrDefaultAsync(a => a.Id == id);
            if (alert is null) return Results.NotFound();

            alert.DispatchStatus = req.Status;
            alert.AssignedOfficer = req.Officer ?? alert.AssignedOfficer;
            if (req.Status is "Closed") { alert.Status = AlertStatus.Resolved; alert.ResolvedAt = DateTime.UtcNow; }
            if (req.Status is "False") { alert.Status = AlertStatus.FalseAlarm; alert.ResolvedAt = DateTime.UtcNow; }

            await db.SaveChangesAsync();
            return Results.Ok(new { alert.Id, alert.DispatchStatus });
        }).RequireAuthorization();

        // 4. Statistika (dashboard yuqorisidagi raqamlar)
        g.MapGet("/stats", async (AppDbContext db) =>
        {
            var today = DateTime.UtcNow.Date;
            var todayCount = await db.Alerts.CountAsync(a => a.SentAt >= today);
            var closed = await db.Alerts.CountAsync(a => a.DispatchStatus == "Closed");
            return Results.Ok(new
            {
                todayCalls = todayCount,
                avgResponse = "4:21",   // mock
                activePatrols = 14,     // mock
                completed = closed,
            });
        }).RequireAuthorization();
    }

    record OfficerLoginRequest(string ServiceId, string? Password);
    record UpdateStatusRequest(string Status, string? Officer);
}
