# FootballSim AAA Prototip (Unity)

Bu repo, tamamen özgün ve lisanssız içeriklerle AAA düzeyinde gerçekçilik hedefleyen bir futbol simülasyonu prototipini tanımlar.

## Klasör Yapısı (Özet)
```
Assets/
  Scripts/
    AI/             -> Pozisyon alma, pres, hata yapabilen AI state machine
    Camera/         -> TV yayını tarzı dinamik kamera
    Career/         -> Basit kariyer sistemi ve rastgele oyuncu üretimi
    Core/           -> State machine ve ortak altyapı
    Data/           -> ScriptableObject veri modelleri
    Gameplay/       -> Oyuncu kontrol, pas/şut/first-touch
    Match/          -> Maç süresi, uzatma, gol olayları
    Physics/        -> Top fiziği (spin/falso/sürtünme/sekme)
    Referee/        -> Basit ama inandırıcı hakem mantığı
Build/
  BuildWindows.ps1  -> Unity batch build komutu
Installer/
  Setup.iss         -> Inno Setup scripti
  BuildInstaller.ps1-> Installer üretim scripti
```

## Başlıca Scriptler
- `PlayerController.cs`: Oyuncu ağırlığı, momentum, stamina etkisi, first-touch kontrolü ve ayak farkı.
- `BallPhysics.cs`: Spin, falso, sürtünme, sekme ve Magnus etkisi.
- `AIStateMachine.cs`: Pres, pozisyon, destek koşuları ve müdahale davranışı.
- `BroadcastCamera.cs`: TV yayını, top hızına göre zoom, ceza sahasında dramatik yakınlaşma.

## Windows Build + Installer (Setup EXE)
> Bu adımlar Windows ortamında ve Unity Editor kurulu iken çalıştırılmalıdır.

1. **Unity build üretimi**
   ```powershell
   ./Build/BuildWindows.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\2022.3.20f1\Editor\Unity.exe"
   ```
   Çıktı: `Builds/Windows/FootballSim.exe`

2. **Installer üretimi (Inno Setup)**
   - Inno Setup 6 kurulu olmalı.
   ```powershell
   ./Installer/BuildInstaller.ps1 -InnoPath "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
   ```
   Çıktı: `Builds/Installer/FootballSim-Setup.exe`

> Tüm içerikler kurgusaldır; gerçek takım, lig, oyuncu veya marka kullanılmaz.
