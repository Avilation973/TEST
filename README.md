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
```

## Başlıca Scriptler
- `PlayerController.cs`: Oyuncu ağırlığı, momentum, stamina etkisi, first-touch kontrolü ve ayak farkı.
- `BallPhysics.cs`: Spin, falso, sürtünme, sekme ve Magnus etkisi.
- `AIStateMachine.cs`: Pres, pozisyon, destek koşuları ve müdahale davranışı.
- `BroadcastCamera.cs`: TV yayını, top hızına göre zoom, ceza sahasında dramatik yakınlaşma.

> Tüm içerikler kurgusaldır; gerçek takım, lig, oyuncu veya marka kullanılmaz.
