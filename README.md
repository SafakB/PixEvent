# PixEvent (Pub/Sub) – Lightweight Event System for Unity

PixEvent is a powerful and flexible **event management system** built for Unity projects. It allows developers to subscribe to events with priority, conditional logic, one-time execution, and synchronous/asynchronous delivery. Its simple structure makes it easy to integrate, extend, and test.

## 🔧 Features (Index)

1. [Event System Introduction](#1-event-system-introduction)
2. [Subscription Mechanisms](#2-subscription-mechanisms)
   - Standard Subscription
   - One-Time Subscription
   - Conditional Subscription
3. [Publishing Events](#3-publishing-events)
4. [Unsubscribing](#4-unsubscribing)
5. [Using Empty Events](#5-using-empty-events)
6. [Logging Support](#6-logging-support)
7. [Helpful Utility Methods](#7-helpful-utility-methods)

---

## 1. Event System Introduction

PixEvent enables communication via any class that implements the `IEvent` interface. This supports a loosely-coupled communication model across systems.

---

## 2. Subscription Mechanisms

### Standard Subscription

```csharp
PixEvent.Subscribe<MyEvent>(OnMyEvent, priority: 10, async: false);
```

Allows setting callback priority and asynchronous execution.

#### Asynchronous Subscription Example

For heavy or long-running tasks, use `async: true` to run the callback on a separate thread:

```csharp
PixEvent.Subscribe<MyEvent>(evt => { 
    Debug.Log("Heavy async process started...");
    Task.Delay(1000).Wait();
    Debug.Log("Heavy async process done.");
}, async: true);
```

> ⚠️ Note: In this example, `Task.Delay().Wait()` is used for demonstration. In Unity environments, using true `async/await` can require additional handling or libraries like UniTask.

---

### One-Time Subscription

```csharp
PixEvent.OneTimeSubscribe<MyEvent>(OnMyEvent);
```

Callback is triggered only once and then unsubscribed automatically.

---

### Conditional Subscription

```csharp
PixEvent.ConditionalSubscribe<MyEvent>(
    condition: e => e.Value > 5,
    callback: OnMyEvent
);
```

Callback is triggered only when the event data satisfies the given condition.

---

## 3. Publishing Events

```csharp
PixEvent.Publish(new MyEvent { Value = 10 }, delayMs: 200);
```

Events can be published with an optional delay. Subscribers are called in descending order of priority.

---

## 4. Unsubscribing

### Remove a Specific Callback

```csharp
PixEvent.Unsubscribe<MyEvent>(OnMyEvent);
```

### Remove All Subscribers

```csharp
PixEvent.UnsubscribeAll<MyEvent>();
```

---

## 5. Using Empty Events

For simple event calls that don't require parameters, use the `EmptyEvent` class:

```csharp
PixEvent.Subscribe(() => Debug.Log("Triggered"));
PixEvent.Publish();
```

---

## 6. Logging Support

Enable logging for internal operations:

```csharp
PixEvent.LogEvents = true;
```

---

## 7. Helpful Utility Methods

### Check for Subscribers

```csharp
if (PixEvent.HasSubscribers<MyEvent>())
{
    // At least one subscriber exists
}
```

---

## 📌 Notes

- Higher `priority` means the callback runs earlier.
- Callbacks marked with `async = true` are executed on separate threads.
- Exceptions are automatically logged via Unity’s `Debug.LogError`.

---

## 👨‍💻 Example Usage

```csharp
public class MyEvent : IEvent
{
    public string Message;
}

// Subscribe
PixEvent.Subscribe<MyEvent>(e => Debug.Log(e.Message));

// Publish
PixEvent.Publish(new MyEvent { Message = "Hello World" });
```

---

This system helps you build more modular, readable, and maintainable code structures.

## 📄 License

This project is licensed under the MIT License. You are free to use, copy, distribute, and modify the software. However, the software is provided "as is", without warranty of any kind.

All rights reserved © 2025 Şafak Bahçe