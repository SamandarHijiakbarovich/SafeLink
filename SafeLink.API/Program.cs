using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeLink.API.Data;
using SafeLink.API.Endpoints;
using SafeLink.API.Models;
using SafeLink.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Ma'lumotlar bazasi — Provider sozlamasiga qarab tanlanadi
// (local dev: Sqlite, server kerak emas; production: PostgreSQL)
var dbProvider = builder.Configuration["Database:Provider"] ?? "Postgres";
var connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        opt.UseSqlite(connString);
    else
        opt.UseNpgsql(connString);
});

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        };
    });

builder.Services.AddAuthorization();

// JSON: EF navigatsiya sikllarini e'tiborsiz qoldirish (User ↔ TrustedContact)
// va enum'larni satr ko'rinishida ("Active"/"Resolved"/"FalseAlarm") yuborish
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// DB sxemasini tayyorlash
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        db.Database.EnsureCreated();   // SQLite: model'dan to'g'ridan-to'g'ri (migration kerak emas)
    else
        db.Database.Migrate();         // PostgreSQL: migration'lar orqali

    // DEV: test foydalanuvchisini bazaga kiritib/yangilab qo'yamiz (har safar ro'yxatdan o'tmaslik uchun)
    if (app.Environment.IsDevelopment())
    {
        const string devPhone = "+998937650083";
        const string devName = "Samandar Mamasoatov Hojiakbar o'g'li";
        var devUser = db.Users.FirstOrDefault(u => u.PhoneNumber == devPhone);
        if (devUser is null)
        {
            db.Users.Add(new User { PhoneNumber = devPhone, FullName = devName, IsVerified = true });
        }
        else
        {
            devUser.FullName = devName;
            devUser.IsVerified = true;
        }
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuth();
app.MapOneId();
app.MapAlerts();
app.MapProfile();
app.MapDispatch();

app.MapGet("/health", () => new { status = "ok", time = DateTime.UtcNow });

app.Run();
