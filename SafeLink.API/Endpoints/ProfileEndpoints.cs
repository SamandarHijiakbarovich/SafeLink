using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SafeLink.API.Data;
using SafeLink.API.Models;

namespace SafeLink.API.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfile(this WebApplication app)
    {
        var g = app.MapGroup("/profile").WithTags("Profile").RequireAuthorization();

        g.MapGet("/", async (ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await db.Users
                .Include(u => u.TrustedContacts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        g.MapPut("/", async (UpdateProfileRequest req, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await db.Users.FindAsync(userId);
            if (user is null) return Results.NotFound();

            user.FullName = req.FullName;
            // Bo'sh JShShIR'ni NULL sifatida saqlaymiz — unique indeks bo'sh qiymatlarda to'qnashmasligi uchun
            user.NationalId = string.IsNullOrWhiteSpace(req.NationalId) ? null : req.NationalId;
            user.ProtectionOrderNumber = req.ProtectionOrderNumber;
            user.ProtectionOrderExpiry = req.ProtectionOrderExpiry;
            user.BloodType = req.BloodType;
            user.Allergies = req.Allergies;
            await db.SaveChangesAsync();

            return Results.Ok(user);
        });

        // Kontakt qo'shish
        g.MapPost("/contacts", async (ContactRequest req, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await db.TrustedContacts.CountAsync(c => c.UserId == userId);
            if (count >= 5) return Results.BadRequest(new { error = "Maksimal 5 ta kontakt" });

            var contact = new TrustedContact
            {
                UserId = userId,
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                Relationship = req.Relationship,
                AvatarColor = req.AvatarColor ?? "#7C3AED",
            };
            db.TrustedContacts.Add(contact);
            await db.SaveChangesAsync();

            return Results.Created($"/profile/contacts/{contact.Id}", contact);
        });

        // Kontakt o'chirish
        g.MapDelete("/contacts/{id:int}", async (int id, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var contact = await db.TrustedContacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (contact is null) return Results.NotFound();

            db.TrustedContacts.Remove(contact);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    record UpdateProfileRequest(string FullName, string NationalId, string? ProtectionOrderNumber,
        DateTime? ProtectionOrderExpiry, string? BloodType, string? Allergies);
    record ContactRequest(string FullName, string PhoneNumber, string Relationship, string? AvatarColor);
}
