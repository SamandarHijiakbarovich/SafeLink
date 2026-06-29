using Microsoft.EntityFrameworkCore;
using SafeLink.API.Data;
using SafeLink.API.Models;
using SafeLink.API.Services;

namespace SafeLink.API.Endpoints;

/// <summary>
/// OneID (id.egov.uz) — davlat yagona identifikatsiya tizimi orqali kirish.
///
/// HOZIR: mock/sandbox — real OneID serveri o'rniga namuna fuqaro qaytaradi.
/// RUXSAT OLINGACH: quyidagi "REAL INTEGRATSIYA" izohidagi qadamlar yoziladi.
/// </summary>
public static class OneIdEndpoints
{
    public static void MapOneId(this WebApplication app)
    {
        var g = app.MapGroup("/auth/oneid").WithTags("OneID");

        // 1. OneID login sahifasining URL manzili
        //    (mobil ilova foydalanuvchini shu manzilga yo'naltiradi)
        g.MapGet("/url", (IConfiguration cfg) =>
        {
            var clientId = cfg["OneId:ClientId"] ?? "SANDBOX";
            var redirect = cfg["OneId:RedirectUri"] ?? "safelink://oneid";
            // Real OneID authorize endpointi:
            var authUrl =
                "https://sso.egov.uz/sso/oauth/Authorization.do" +
                $"?response_type=one_code&client_id={clientId}" +
                $"&redirect_uri={Uri.EscapeDataString(redirect)}" +
                "&scope=safelink&state=app";
            return Results.Ok(new { authUrl });
        });

        // 2. OneID callback — authorization code'ni token + profilga almashtiradi
        g.MapPost("/callback", async (OneIdCallbackRequest req, AppDbContext db, TokenService tokens) =>
        {
            // ┌─ REAL INTEGRATSIYA (ruxsat olingach) ───────────────────────────────
            // 1) access_token olish:
            //    POST https://sso.egov.uz/sso/oauth/Authorization.do
            //    grant_type=one_authorization_code & code={req.Code}
            //    & client_id={..} & client_secret={..} & redirect_uri={..}
            //    → { access_token }
            // 2) Shaxs ma'lumotini olish:
            //    POST https://sso.egov.uz/sso/oauth/Authorization.do
            //    grant_type=one_access_token_identify & access_token={..}
            //    & scope=safelink & client_id={..} & client_secret={..}
            //    → { pin (JShShIR/PINFL), sur_name, first_name, mid_name, birth_date, ... }
            // └─────────────────────────────────────────────────────────────────────

            if (string.IsNullOrWhiteSpace(req.Code))
                return Results.BadRequest(new { error = "Authorization code yo'q" });

            // MOCK: real OneID javobi o'rniga sandbox fuqaro ma'lumoti
            var citizen = MockCitizen(req.Code);

            // JShShIR (PINFL) bo'yicha foydalanuvchini topamiz yoki yaratamiz
            var user = await db.Users.FirstOrDefaultAsync(u => u.NationalId == citizen.Pinfl);
            bool isNew = user is null;
            if (isNew)
            {
                user = new User
                {
                    PhoneNumber = citizen.Phone,
                    FullName = citizen.FullName,
                    NationalId = citizen.Pinfl,
                    IsVerified = true,   // OneID — davlat tasdiqlagan shaxs
                };
                db.Users.Add(user);
            }
            else
            {
                // Davlat ma'lumoti bilan sinxronlaymiz
                user!.FullName = citizen.FullName;
                user.IsVerified = true;
            }
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                token = tokens.Generate(user!),
                isNewUser = isNew,
                userId = user!.Id,
                fullName = user.FullName,
                nationalId = user.NationalId,
            });
        });
    }

    // Sandbox uchun namuna fuqaro (real OneID javobini taqlid qiladi).
    // Bir xil code → bir xil fuqaro, shuning uchun qayta kirishda o'sha hisob topiladi.
    static OneIdCitizen MockCitizen(string code) =>
        new(
            Pinfl: "51905901234567",
            FullName: "Karimova Madina Akmal qizi",
            Phone: "+998901112233",
            BirthDate: new DateTime(1990, 5, 12));

    record OneIdCallbackRequest(string? Code, string? State);
    record OneIdCitizen(string Pinfl, string FullName, string Phone, DateTime BirthDate);
}
