# SafeLink

Favqulodda holatlarda yordam chaqirish uchun mobil ilova. Foydalanuvchi SOS tugmasini bosib, joylashuv, audio yozuv va ishonchli kontaktlar orqali tezkor yordam ola oladi.

## Texnologiyalar

- **.NET 9** + **.NET MAUI** — multiplatform mobil ilova
- **MVVM** arxitektura — `CommunityToolkit.Mvvm`
- **DI** — built-in `Microsoft.Extensions.DependencyInjection`
- **XAML** + C# kodi
- **CommunityToolkit.Maui** — qo'shimcha controllar va konverterlar

## Loyiha tuzilishi

```
SafeLink/
├── Models/              # Ma'lumot sinflari
│   ├── User.cs
│   ├── TrustedContact.cs
│   ├── AlertEvent.cs
│   └── BrelokDevice.cs
├── Services/            # Biznes mantiq
│   ├── EmergencyService.cs
│   └── GeolocationService.cs
├── ViewModels/          # MVVM ViewModel lar
│   ├── HomeViewModel.cs
│   ├── SosHoldViewModel.cs
│   ├── AlertSentViewModel.cs
│   └── ProfileViewModel.cs
├── Views/               # XAML sahifalar
│   ├── HomePage.xaml
│   ├── SosHoldPage.xaml
│   ├── AlertSentPage.xaml
│   ├── ProfilePage.xaml
│   └── PairingPage.xaml
├── Converters/          # XAML konverterlar
└── Resources/           # Ranglar, fontlar, ikonkalar
```

## Ekranlar

| # | Ekran | Tavsif |
|---|-------|--------|
| 1 | **Bosh sahifa** | Qora-ko'k fon, pulslanuvchi qizil SOS tugmasi |
| 2 | **SOS Hold** | 3 soniya bosib turish — countdown |
| 3 | **Alert Sent** | Tasdiqlash, xarita, politsiya ETA |
| 4 | **Profil** | Foydalanuvchi, brelok, kontaktlar |
| 5 | **Pairing** | Bluetooth brelok ulash |

## Imkoniyatlar

- SOS tugmasi (3 soniya bosib turish)
- GPS joylashuvni aniqlash
- Audio yozuv (favqulodda paytida)
- 3 ishonchli kontaktga avtomatik xabar
- 102 raqamiga to'g'ridan-to'g'ri ulanish
- SafeLink Brelok SL-100 Bluetooth orqali ulash
- Himoya orderi tarixi

## O'rnatish

### Talablar
- .NET 9 SDK
- Visual Studio 2022 (MAUI workload bilan)
- Android SDK (telefon yoki emulyator uchun)

### Build qilish
```bash
# Loyihani qurish
dotnet build -f net9.0-android

# APK yaratish (telefon uchun)
dotnet publish -f net9.0-android -c Debug -p:EmbedAssembliesIntoApk=true

# APK ni telefonga o'rnatish
adb install bin/Debug/net9.0-android/*-Signed.apk
```

## Ranglar tizimi

Qizil rang **faqat** SOS va xavf elementlari uchun ishlatiladi:

| Rang | HEX | Maqsad |
|------|-----|--------|
| Navy | `#0A2540` | Brend, ishonch |
| Red | `#E11D48` | SOS, xavf |
| Green | `#16A34A` | Muvaffaqiyat |
| BgMain | `#F5F7FB` | Asosiy fon |

## Litsenziya

MIT
