using SafeLink.API.Models;

namespace SafeLink.API.Data;

public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        // Demo foydalanuvchilar (IIV taqdimoti uchun)
        if (!db.Users.Any())
        {
            var users = new[]
            {
                new User { PhoneNumber = "+998937650083", FullName = "Nilufar Yusupova", IsVerified = true, OrderNumber = "HO-2024-001234" },
                new User { PhoneNumber = "+998901234567", FullName = "Malika Toshmatova", IsVerified = true, OrderNumber = "HO-2024-005678" },
                new User { PhoneNumber = "+998991112233", FullName = "Barno Karimova", IsVerified = true, OrderNumber = "HO-2024-009012" },
            };
            db.Users.AddRange(users);
            db.SaveChanges();

            // Ishonchli kontaktlar
            db.TrustedContacts.AddRange(
                new TrustedContact { UserId = users[0].Id, FullName = "Onasi — Xurmo Yusupova", PhoneNumber = "+998901110001", Relationship = "Onasi" },
                new TrustedContact { UserId = users[0].Id, FullName = "Akasi — Jahongir Yusupov", PhoneNumber = "+998901110002", Relationship = "Akasi" },
                new TrustedContact { UserId = users[1].Id, FullName = "Opasi — Gulnora", PhoneNumber = "+998901110003", Relationship = "Opasi" }
            );

            // Demo hodisalar (tarix uchun)
            db.Alerts.AddRange(
                new Alert
                {
                    UserId = users[0].Id, Status = AlertStatus.Resolved,
                    DispatchStatus = "Closed", AssignedOfficer = "Kpr. Sotvoldiyev A.",
                    Latitude = 41.2995, Longitude = 69.2401,
                    Address = "Mirzo Ulug'bek t., 15-uy, Toshkent",
                    SentAt = DateTime.UtcNow.AddDays(-3), ResolvedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(7),
                    PoliceEtaMinutes = 4,
                },
                new Alert
                {
                    UserId = users[0].Id, Status = AlertStatus.FalseAlarm,
                    DispatchStatus = "False",
                    Latitude = 41.3111, Longitude = 69.2801,
                    Address = "Yunusobod t., Navruz park yaqini",
                    SentAt = DateTime.UtcNow.AddDays(-10), ResolvedAt = DateTime.UtcNow.AddDays(-10).AddMinutes(2),
                    PoliceEtaMinutes = 5,
                },
                new Alert
                {
                    UserId = users[1].Id, Status = AlertStatus.Active,
                    DispatchStatus = "EnRoute", AssignedOfficer = "Srj. Nazarov B.",
                    Latitude = 41.2755, Longitude = 69.2099,
                    Address = "Chilonzor t., Buyuk Ipak Yo'li ko'chasi, 84",
                    SentAt = DateTime.UtcNow.AddMinutes(-8),
                    PoliceEtaMinutes = 3,
                }
            );
            db.SaveChanges();
        }
    }
}
