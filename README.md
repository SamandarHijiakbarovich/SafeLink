<div align="center">

# 🛡 SafeLink

**Favqulodda holatlarda hayot saqlaydigan mobil ilova**

Bir tugma — tezkor yordam. Joylashuv, audio yozuv va ishonchli kontaktlar bir necha soniyada xabardor bo'ladi.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![MAUI](https://img.shields.io/badge/.NET_MAUI-9.0-purple?style=flat-square)
![Android](https://img.shields.io/badge/Android-21%2B-3DDC84?style=flat-square&logo=android)
![iOS](https://img.shields.io/badge/iOS-15%2B-000000?style=flat-square&logo=apple)
![License](https://img.shields.io/badge/License-MIT-blue?style=flat-square)

</div>

---

## 📖 Loyiha haqida

**SafeLink** — bu xavfli vaziyatlarga tushgan, ayniqsa oilaviy zo'ravonlik qurboni bo'lgan ayollar uchun mo'ljallangan favqulodda yordam ilovasi. Foydalanuvchi telefonda yoki maxsus brelokda bitta tugmani bosib turishi bilan:

- 📍 Joylashuvi GPS orqali aniqlanadi
- 🎙 Atrofdagi tovushlar yozib olinadi
- 📞 102 raqamiga avtomatik murojaat qilinadi
- 👥 Ishonchli kontaktlar (oila a'zolari) xabardor qilinadi
- 🚓 Eng yaqin patrul yo'lga chiqadi

Barchasi — **3 soniya ichida**.

### 💡 Nima uchun bu kerak?

O'zbekistonda har yili minglab ayollar oilaviy zo'ravonlik qurboni bo'lmoqda. Mavjud "102" tizimi telefonda gaplashishni talab qiladi, bu esa qurbon uchun har doim ham mumkin emas. SafeLink — **jim, tezkor va aniq** yordam chaqirish vositasi.

---

## ✨ Asosiy imkoniyatlar

### 🔴 SOS signal
- 3 soniya bosib turish — tasodifiy bosilishdan himoya
- Animatsion countdown + haptik javob
- Bekor qilish imkoniyati (yolg'on signal)

### 📍 Joylashuv
- GPS bilan ±4 metr aniqlik
- Manzilni avtomatik aniqlash (reverse geocoding)
- Patrul ETA hisoblash

### 🎙 Audio yozuv
- Signal yuborilishi bilanoq avtomatik yoqiladi
- Real vaqtda serverga uzatiladi
- Sud uchun dalil sifatida saqlanadi

### 👥 Ishonchli kontaktlar
- 3 tagacha oila a'zosi
- SMS + push notification
- Joylashuv link bilan

### 🔑 Bluetooth Brelok (SL-100)
- Maxsus tashqi qurilma
- Telefoni yo'q joyda ham ishlaydi
- Bluetooth LE — kam quvvat sarflaydi
- Batareya holati ilovada ko'rinadi

### 🛡 Himoya orderi
- Sud tomonidan berilgan order ma'lumotlari
- Amal qilish muddati
- IIB filiali ma'lumotlari

---

## 🏗 Arxitektura

Loyihada **MVVM (Model-View-ViewModel)** patterni ishlatilgan:

```
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────┐
│      View       │◀────▶│    ViewModel     │◀────▶│      Model      │
│  (XAML sahifa)  │      │ (UI mantiq)      │      │  (Ma'lumotlar)  │
└─────────────────┘      └──────────────────┘      └─────────────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │    Services      │
                         │ (Biznes mantiq)  │
                         └──────────────────┘
```

**Foydalari:**
- 🧪 Test qilish oson (har bir qatlam mustaqil)
- 🔄 UI va mantiq ajratilgan
- 📦 Code reuse — bir ViewModel turli sahifalarda
- 🛠 Yangi funksiya qo'shish oson

---

## 🛠 Texnologiyalar

| Kategoriya | Texnologiya | Vazifa |
|------------|-------------|--------|
| **Platform** | .NET 9 + .NET MAUI | Android, iOS, Windows uchun bitta kod |
| **UI** | XAML + C# | Deklarativ interfeys |
| **MVVM** | CommunityToolkit.Mvvm | `[ObservableProperty]`, `[RelayCommand]` |
| **DI** | Microsoft.Extensions.DI | Dependency Injection |
| **Toolkit** | CommunityToolkit.Maui | Konverterlar, popups, animatsiyalar |
| **GPS** | Microsoft.Maui.Essentials | Geolocation, Geocoding |
| **Permissions** | Microsoft.Maui.Essentials | Runtime ruxsatlar |
| **Phone** | PhoneDialer | 102 ga qo'ng'iroq |

---

## 📁 Loyiha tuzilishi

```
SafeLink/
├── 📂 Models/                  # Ma'lumot sinflari (POCO)
│   ├── User.cs                 # Foydalanuvchi: ism, JShShIR, himoya orderi
│   ├── TrustedContact.cs       # Ishonchli kontakt: ism, munosabat, raqam
│   ├── AlertEvent.cs           # SOS hodisasi: vaqt, joylashuv, holat
│   └── BrelokDevice.cs         # Bluetooth qurilma: ulanish, batareya
│
├── 📂 Services/                # Biznes mantiq qatlami
│   ├── 📂 Interfaces/
│   │   ├── IEmergencyService.cs
│   │   └── IGeolocationService.cs
│   ├── EmergencyService.cs     # SOS yuborish jarayonini boshqaradi
│   └── GeolocationService.cs   # GPS bilan ishlash
│
├── 📂 ViewModels/              # UI mantiq qatlami (MVVM)
│   ├── BaseViewModel.cs        # Umumiy: IsBusy, Title
│   ├── HomeViewModel.cs        # Bosh sahifa
│   ├── SosHoldViewModel.cs     # 3 soniya countdown
│   ├── AlertSentViewModel.cs   # Tasdiqlash
│   └── ProfileViewModel.cs     # Profil va kontaktlar
│
├── 📂 Views/                   # XAML sahifalar
│   ├── HomePage.xaml(.cs)      # Pulslanuvchi SOS tugma
│   ├── SosHoldPage.xaml(.cs)   # Qizil ekran, countdown
│   ├── AlertSentPage.xaml(.cs) # Xarita, status, kontaktlar
│   ├── ProfilePage.xaml(.cs)   # Foydalanuvchi ma'lumotlari
│   ├── PairingPage.xaml(.cs)   # Brelok ulash
│   └── HistoryPage.xaml(.cs)   # Tarix
│
├── 📂 Converters/              # XAML konverterlar
│   └── BoolToColorConverter.cs # bool → Color
│
├── 📂 Resources/
│   ├── AppIcon/                # Ilova ikonkasi (SVG)
│   ├── Splash/                 # Splash screen
│   ├── Styles/
│   │   ├── Colors.xaml         # SafeLink ranglar tizimi
│   │   └── Styles.xaml         # MAUI standart uslublar
│   └── Fonts/
│
├── 📂 Platforms/
│   ├── Android/
│   │   ├── AndroidManifest.xml # Ruxsatlar
│   │   ├── MainActivity.cs
│   │   └── MainApplication.cs
│   └── iOS/
│       ├── Info.plist
│       └── AppDelegate.cs
│
├── App.xaml(.cs)               # Application kirish nuqtasi
├── AppShell.xaml(.cs)          # Navigatsiya + tab bar
├── MauiProgram.cs              # DI sozlash
└── SafeLink.csproj             # Loyiha sozlamalari
```

---

## 📱 Ekranlar

### 1. Bosh sahifa
- **Fon:** Qora-ko'k gradient (navy)
- **Markaz:** Pulslanuvchi qizil SOS tugmasi
- **Yuqori:** Salomlashuv + brelok holati
- **Pastki:** Himoya orderi kartasi + tab bar

### 2. SOS Hold
- **Fon:** Qizil gradient
- **Markaz:** Countdown halqa (3 → 2 → 1 → 0)
- **Status:** Joylashuv, audio, 102 ulanish
- **Pastki:** Bekor qilish tugmasi

### 3. Alert Sent
- **Yuqori:** Yashil ✓ + "Yuborildi" matni
- **Markaz:** Xarita + manzil chip
- **Status:** Politsiya ETA, kontaktlar, audio
- **Pastki:** Operator bilan bog'lanish

### 4. Profil
- **Identifikatsiya kartasi:** Avatar, ism, JShShIR
- **Himoya orderi:** Raqam, muddat
- **Qurilma:** Brelok ma'lumotlari
- **Kontaktlar:** 3 ishonchli kontakt
- **Sozlamalar:** Tibbiy, ovoz, til, hujjatlar

### 5. Brelok ulash
- **Bluetooth qidirish** animatsiyasi
- **Qurilma kartasi** signal va masofa bilan
- **3 qadam:** qidirish → topish → ulash

---

## 🎨 Dizayn tizimi

### Ranglar

> ⚠️ **Qoida:** Qizil rang **faqat** SOS va xavf elementlari uchun. Boshqa joyda ishlatmaslik.

| Maqsad | Rang | HEX |
|--------|------|-----|
| **Navy** (brend, ishonch) | 🟦 | `#0A2540` |
| **Navy 2** | 🟦 | `#13325F` |
| **Navy 3** | 🟦 | `#1F4480` |
| **Red** (SOS, xavf) | 🔴 | `#E11D48` |
| **Red Deep** | 🔴 | `#9F1239` |
| **Green** (muvaffaqiyat) | 🟢 | `#16A34A` |
| **Amber** (ogohlantirish) | 🟡 | `#D97706` |
| **Bg Main** (fon) | ⚪ | `#F5F7FB` |
| **Ink** (matn) | ⚫ | `#0A2540` |
| **Muted** (yordamchi matn) | ⚪ | `#64748B` |

### Tipografiya

- **Family:** `Manrope` (Google Fonts) → MAUI default
- **Weights:** 400 (regular), 600 (semi-bold), 700 (bold), 800 (extra-bold)

### Spacing

- **Page padding:** 16-24px
- **Component padding:** 14-20px
- **Element gap:** 4, 8, 12, 16, 24px
- **Border radius:** 10-22px (cards), 999px (pills)

---

## 🔒 Android ruxsatlari

`Platforms/Android/AndroidManifest.xml` da quyidagi ruxsatlar so'raladi:

| Ruxsat | Maqsad |
|--------|--------|
| `ACCESS_FINE_LOCATION` | GPS joylashuv |
| `ACCESS_COARSE_LOCATION` | Tarmoq joylashuv |
| `BLUETOOTH_SCAN` | Brelokni qidirish |
| `BLUETOOTH_CONNECT` | Brelokga ulanish |
| `RECORD_AUDIO` | Audio yozuv |
| `CALL_PHONE` | 102 ga qo'ng'iroq |
| `VIBRATE` | Haptik javob |
| `WAKE_LOCK` | SOS paytida ekran yoniq |
| `INTERNET` | Server bilan ulanish |

---

## 🚀 O'rnatish va ishga tushirish

### Talablar

- **.NET 9 SDK** — [Yuklab olish](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** (Community ham bo'ladi) + **.NET MAUI workload**
- **Android SDK** (Visual Studio bilan birga) — yoki haqiqiy Android telefon

### Klon qilish

```bash
git clone https://github.com/dotnetdasturchi/SafeLink.git
cd SafeLink
```

### NuGet paketlarni tiklash

```bash
dotnet restore
```

### Build

```bash
# Faqat tekshirish
dotnet build -f net9.0-android

# Telefonga o'rnatish uchun APK
dotnet publish -f net9.0-android -c Debug -p:EmbedAssembliesIntoApk=true
```

### APK ni telefonga o'rnatish

```bash
# USB orqali ulangan telefonni tekshirish
adb devices

# APK ni o'rnatish
adb install bin/Debug/net9.0-android/uz.safelink.app-Signed.apk

# Ishga tushirish
adb shell monkey -p uz.safelink.app -c android.intent.category.LAUNCHER 1
```

---

## 🗺 Yo'l xaritasi (Roadmap)

### ✅ 1-bosqich: MVP (joriy)
- [x] Asosiy ekranlar (Home, SOS, Profile, Pairing)
- [x] MVVM arxitektura
- [x] DI orqali servislar
- [x] Android ruxsatlari
- [x] Dizayn tizimi

### 🔄 2-bosqich: Backend integratsiya
- [ ] REST API (.NET Core Web API)
- [ ] PostgreSQL ma'lumotlar bazasi
- [ ] JWT autentifikatsiya
- [ ] SignalR — real vaqt push
- [ ] Audio storage (Azure Blob / S3)
- [ ] SMS gateway integratsiyasi

### 🔄 3-bosqich: IIV xodimi ilovasi
- [ ] Yangi MAUI loyihasi (SafeLink.Officer)
- [ ] Yaqin atrofdagi signallar (xarita + ro'yxat)
- [ ] Signalga javob berish jarayoni
- [ ] Real vaqt navigatsiya
- [ ] Voqea bayonnomasi yaratish
- [ ] Hisobotlar

### 🔄 4-bosqich: Admin paneli (Web)
- [ ] Blazor Server / Angular dashboard
- [ ] Foydalanuvchilar boshqaruvi
- [ ] Statistika va hisobotlar
- [ ] Xodimlarni biriktirish
- [ ] Tizim sozlamalari

### 🔄 5-bosqich: Qo'shimcha imkoniyatlar
- [ ] Tibbiy ma'lumotlar to'ldirish
- [ ] Hujjat va arizalarni jo'natish
- [ ] Operatorlar bilan ichki chat
- [ ] Push notification rejimi (jim/oddiy)
- [ ] Ko'p tilli (O'zbek lotin/kirill, Rus)
- [ ] iOS versiyasi

---

## 🤝 Hissa qo'shish

Loyiha hozircha shaxsiy rivojlanish bosqichida. Takliflar, savollar yoki bug hisobotlari uchun [Issues](https://github.com/dotnetdasturchi/SafeLink/issues) ochishingiz mumkin.

---

## 📞 Aloqa

- 📧 Email: dotnetdasturchi@gmail.com
- 💬 GitHub: [@dotnetdasturchi](https://github.com/dotnetdasturchi)

---

## 📄 Litsenziya

Bu loyiha **MIT** litsenziyasi ostida tarqatiladi. Batafsil: [LICENSE](LICENSE) fayli.

---

<div align="center">

**SafeLink** — har bir hayot qadrlidir 💙

Made with ❤️ in O'zbekiston · .NET MAUI bilan

</div>
