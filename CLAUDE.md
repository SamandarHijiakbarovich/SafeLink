# SafeLink — Loyiha Konteksti

## Loyiha haqida
SafeLink — ayollarni zo'ravonlikdan himoya qilish uchun favqulodda yordam ilovasi.
Bir tugma bosish bilan 3 soniyada: GPS + audio + SMS kontaktlarga + 102 ga qo'ng'iroq.

## Stack
- **Mobil:** .NET 9 + MAUI (Android/iOS), MVVM arxitekturasi, CommunityToolkit.Mvvm
- **Backend:** ASP.NET Core Minimal API (.NET 9), PostgreSQL, EF Core, JWT auth
- **Arxitektura:** MVVM — View → ViewModel → Service → Model

## Papka tuzilmasi
```
SafeLink/           ← MAUI mobil ilova
  Models/           ← User, AlertEvent, TrustedContact, BrelokDevice
  Services/         ← EmergencyService, GeolocationService, AuthService,
                       SafeApiClient, BluetoothService
  ViewModels/       ← Home, SosHold, AlertSent, Profile, History, Pairing
  ViewModels/Onboarding/ ← Welcome, Phone, Identity, Order, Contacts, Complete
  Views/            ← MAUI XAML sahifalar
  Views/Onboarding/ ← 6 ta onboarding ekran
  Resources/Styles/ ← Colors.xaml (design tokens), Styles.xaml

SafeLink.API/       ← Backend server
  Program.cs        ← Minimal API entry, JWT, Swagger, auto-migration
  Data/             ← AppDbContext (PostgreSQL)
  Models/           ← User, Alert, TrustedContact, OtpCode
  Endpoints/        ← AuthEndpoints, AlertEndpoints, ProfileEndpoints
  Services/         ← TokenService (JWT), SmsService (Eskiz.uz)
  appsettings.json  ← DB connection, JWT secret, SMS config
```

## Navigatsiya oqimi
```
App ochiladi → token bor? → //main (HomePage)
                          → //welcome (onboarding)

Onboarding: welcome → phone (OTP) → identity → order → contacts → complete → //main

Main: TabBar (home / history / profile)
SOS oqimi: homepage → soshold → alertsent
```

## API Endpoints
| Method | URL | Tavsif |
|--------|-----|--------|
| POST | /auth/send-otp | SMS OTP yuborish |
| POST | /auth/verify-otp | OTP tasdiqlash → JWT |
| POST | /alerts/send | SOS signal (auth kerak) |
| PATCH | /alerts/cancel/{id} | Bekor qilish |
| GET | /alerts/history | Tarix |
| GET | /profile | Profil |
| PUT | /profile | Yangilash |
| POST | /profile/contacts | Kontakt qo'shish |
| DELETE | /profile/contacts/{id} | Kontakt o'chirish |

## Dizayn tizimi
- **Navy #0A2540** — asosiy fon (ishonch rangi)
- **Red #E11D48** — FAQAT SOS tugmasi uchun
- **Green #16A34A** — muvaffaqiyat
- **Amber #D97706** — ogohlantirish
- **Font:** Manrope (400/600/700/800)
- Dizayn fayllari: `/tmp/safelink_design/` (zip dan olingan JSX fayllar)

## Git
- Branch: `claude/adoring-ritchie-45nj8x`
- Repo: `https://github.com/SamandarHijiakbarovich/SafeLink`

## Build qilish (lokal)
```bash
# Backend
cd SafeLink.API
# appsettings.json da PostgreSQL parolini kiriting
dotnet ef migrations add InitialCreate  # birinchi marta
dotnet run  # → http://localhost:5000/swagger

# Mobil APK
cd SafeLink
dotnet workload install maui-android  # birinchi marta
dotnet build -f net9.0-android -c Debug
# APK: bin/Debug/net9.0-android/com.companyname.safelink-Signed.apk
```

## Muhim eslatmalar
- `BluetoothService` hozir mock — real `Plugin.BLE` keyinroq ulanadi
- `SmsService` hozir log ga yozadi — real Eskiz.uz API keyinroq
- `SafeApiClient.cs` da `BaseUrl = "http://10.0.2.2:5000"` (Android emulator → localhost)
- Real telefonda test qilganda `BaseUrl` ni server IP ga o'zgartiring

## Keyingi bosqichlar (Roadmap)
- [ ] Real Bluetooth (Plugin.BLE) integratsiyasi
- [ ] Eskiz.uz SMS gateway ulash
- [ ] Audio yozib saqlash
- [ ] Push notification (FCM)
- [ ] SignalR — real-time politsiya xabardorligi
- [ ] Admin dashboard (Blazor)
- [ ] Politsiya officer ilovasi
