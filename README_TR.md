# PixEvent (Pub/Sub) – Lightweight Event System for Unity

PixEvent, Unity projelerinde kullanılmak üzere geliştirilmiş, güçlü ve esnek bir **event (olay) yönetim sistemidir**. Geliştiricilere farklı önceliklere göre abonelik (subscribe), koşullu abonelik, sadece bir kere çalışacak event'ler tanımlama ve senkron/asenkron yayınlama (publish) desteği sunar. Kod yapısının basitliği sayesinde hızlı entegre edilir, özelleştirilebilir ve test edilebilir bir altyapı sağlar.

## 🔧 Özellikler (Index)

1. [Event Sistemi Giriş](#1-event-sistemi-giriş)
2. [Abonelik İşlemleri](#2-abonelik-işlemleri)
   - Normal Abonelik
   - Tek Seferlik Abonelik
   - Koşullu Abonelik
3. [Event Yayınlama (Publish)](#3-event-yayınlama-publish)
4. [Abonelikten Çıkma (Unsubscribe)](#4-abonelikten-çıkma-unsubscribe)
5. [Boş Event Kullanımı](#5-boş-event-kullanımı)
6. [Loglama Desteği](#6-loglama-desteği)
7. [Yararlı Kontrol Metotları](#7-yararlı-kontrol-metotları)

---

## 1. Event Sistemi Giriş

PixEvent, `IEvent` arayüzünü implemente eden her türdeki sınıf üzerinden olay iletişimi yapılmasına olanak tanır. Bu yapı sayesinde sistemler arası loosely-coupled (gevşek bağlı) bir iletişim modeli kurulabilir.

---

## 2. Abonelik İşlemleri

### Normal Abonelik

```csharp
PixEvent.Subscribe<MyEvent>(OnMyEvent, priority: 10, async: false);
```

Abonelik önceliği ve asenkron çalışıp çalışmayacağı belirlenebilir.

#### Asenkron Abonelik Örneği

Ağır veya uzun süren işlemler için `async: true` kullanılarak işlem ayrı bir iş parçacığında çalıştırılabilir:

```csharp
PixEvent.Subscribe<MyEvent>(evt => { 
    Debug.Log("Ağır async işlem başladı...");
    Task.Delay(1000).Wait();
    Debug.Log("Ağır async işlem tamamlandı.");
}, async: true);
```

> ⚠️ Bu örnekte `Task.Delay().Wait()` kullanılmıştır ama Unity içinde `await`/`async` doğrudan kullanılmazsa dikkatli olunmalıdır. Alternatif olarak `UniTask` gibi kütüphaneler kullanılabilir.

---

### Tek Seferlik Abonelik

```csharp
PixEvent.OneTimeSubscribe<MyEvent>(OnMyEvent);
```

Callback sadece bir kere çalışır ve ardından abonelik kaldırılır.

---

### Koşullu Abonelik

```csharp
PixEvent.ConditionalSubscribe<MyEvent>(
    condition: e => e.Value > 5,
    callback: OnMyEvent
);
```

Event verileri belli koşulları sağlıyorsa callback çalıştırılır.

---

## 3. Event Yayınlama (Publish)

```csharp
PixEvent.Publish(new MyEvent { Value = 10 }, delayMs: 200);
```

Event'ler gecikmeli olarak da gönderilebilir. Abonelikler öncelik sırasına göre çağrılır.

---

## 4. Abonelikten Çıkma (Unsubscribe)

### Belirli Callback'i Kaldırma

```csharp
PixEvent.Unsubscribe<MyEvent>(OnMyEvent);
```

### Tüm Abonelikleri Kaldırma

```csharp
PixEvent.UnsubscribeAll<MyEvent>();
```

---

## 5. Boş Event Kullanımı

Hiçbir parametre gerektirmeyen olaylar için `EmptyEvent` sınıfı kullanılabilir:

```csharp
PixEvent.Subscribe(() => Debug.Log("Triggered"));
PixEvent.Publish();
```

---

## 6. Loglama Desteği

Sistemin içindeki işlemleri loglamak için `LogEvents` özelliğini aktif hale getirin:

```csharp
PixEvent.LogEvents = true;
```

---

## 7. Yararlı Kontrol Metotları

### Abonelik Var mı Kontrolü

```csharp
if (PixEvent.HasSubscribers<MyEvent>())
{
    // En az bir dinleyici var
}
```

---

## 📌 Notlar

- `Priority` değeri ne kadar yüksekse, callback o kadar önce çalışır.
- `async = true` olarak işaretlenen abonelikler ayrı bir thread’de çalıştırılır.
- Exception durumları otomatik olarak loglanır (Unity `Debug.LogError`).

---

## 👨‍💻 Örnek Kullanım

```csharp
public class MyEvent : IEvent
{
    public string Message;
}

// Abone ol
PixEvent.Subscribe<MyEvent>(e => Debug.Log(e.Message));

// Yayınla
PixEvent.Publish(new MyEvent { Message = "Hello World" });
```

---

Bu sistem ile birlikte kodlarınızı daha modüler, okunabilir ve genişletilebilir hale getirebilirsiniz.

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır. Dilediğiniz gibi kullanabilir, kopyalayabilir, dağıtabilir ve değiştirebilirsiniz. Ancak yazılım "olduğu gibi" sunulmaktadır ve hiçbir garanti verilmemektedir.

Tüm hakları saklıdır © 2025 Şafak Bahçe