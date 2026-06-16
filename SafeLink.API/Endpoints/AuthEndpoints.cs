using Microsoft.EntityFrameworkCore;
using SafeLink.API.Data;
using SafeLink.API.Models;
using SafeLink.API.Services;

namespace SafeLink.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuth(this WebApplication app)
    {
        var g = app.MapGroup("/auth").WithTags("Auth");

        // 1. OTP yuborish
        g.MapPost("/send-otp", async (SendOtpRequest req, AppDbContext db, SmsService sms) =>
        {
            // Eski kodlarni o'chirish
            var old = await db.OtpCodes.Where(o => o.PhoneNumber == req.Phone && !o.IsUsed).ToListAsync();
            db.OtpCodes.RemoveRange(old);

            var code = Random.Shared.Next(100000, 999999).ToString();
            db.OtpCodes.Add(new OtpCode
            {
                PhoneNumber = req.Phone,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            });
            await db.SaveChangesAsync();
            await sms.SendOtpAsync(req.Phone, code);

            return Results.Ok(new { message = "OTP yuborildi" });
        });

        // 2. OTP tasdiqlash + JWT olish
        g.MapPost("/verify-otp", async (VerifyOtpRequest req, AppDbContext db, TokenService tokens) =>
        {
            var otp = await db.OtpCodes
                .Where(o => o.PhoneNumber == req.Phone && o.Code == req.Code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (otp is null) return Results.BadRequest(new { error = "Kod noto'g'ri yoki muddati o'tgan" });

            otp.IsUsed = true;

            var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == req.Phone);
            bool isNew = user is null;
            if (isNew)
            {
                user = new User { PhoneNumber = req.Phone };
                db.Users.Add(user);
            }

            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                token = tokens.Generate(user!),
                isNewUser = isNew,
                userId = user!.Id,
            });
        });
    }

    record SendOtpRequest(string Phone);
    record VerifyOtpRequest(string Phone, string Code);
}
