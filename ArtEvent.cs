using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public static class PixEvent
{
    private class EventSubscriber
    {
        public Delegate Callback;
        public int Priority;
        public bool IsAsync;
        public bool OneTime;
        public Func<object, bool> Condition;
    }

    private static readonly Dictionary<Type, List<EventSubscriber>> _subscribers = new();

    public static bool LogEvents { get; set; } = false;

    public static void Subscribe<T>(Action<T> callback, int priority = 0, bool async = false) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<EventSubscriber>();

        _subscribers[type].Add(new EventSubscriber
        {
            Callback = callback,
            Priority = priority,
            IsAsync = async
        });

        if (LogEvents)
            Debug.Log($"[PixEvent] Subscribed to {type.Name} (Priority: {priority}, Async: {async})");
    }

    public static void OneTimeSubscribe<T>(Action<T> callback, int priority = 0, bool async = false) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<EventSubscriber>();

        _subscribers[type].Add(new EventSubscriber
        {
            Callback = callback,
            Priority = priority,
            IsAsync = async,
            OneTime = true
        });

        if (LogEvents)
            Debug.Log($"[PixEvent] OneTimeSubscribed to {type.Name}");
    }

    public static void ConditionalSubscribe<T>(Func<T, bool> condition, Action<T> callback, int priority = 0, bool async = false) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<EventSubscriber>();

        _subscribers[type].Add(new EventSubscriber
        {
            Callback = callback,
            Priority = priority,
            IsAsync = async,
            Condition = obj => condition((T)obj)
        });

        if (LogEvents)
            Debug.Log($"[PixEvent] ConditionalSubscribed to {type.Name}");
    }

    public static void Unsubscribe<T>(Action<T> callback) where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].RemoveAll(s => s.Callback.Equals(callback));
            if (LogEvents)
                Debug.Log($"[PixEvent] Unsubscribed from {type.Name}");
        }
    }

    public static void UnsubscribeAll<T>() where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Clear();
            if (LogEvents)
                Debug.Log($"[PixEvent] Cleared all subscribers of {type.Name}");
        }
    }

    public static bool HasSubscribers<T>() where T : IEvent
    {
        var type = typeof(T);
        return _subscribers.ContainsKey(type) && _subscribers[type].Count > 0;
    }

    public static async void Publish<T>(T data, int delayMs = 0) where T : IEvent
    {
        var type = typeof(T);
        if (LogEvents)
            Debug.Log($"[PixEvent] Publishing {type.Name} (Delay: {delayMs}ms) → Data: {data}");

        if (!_subscribers.ContainsKey(type))
            return;

        if (delayMs > 0)
            await Task.Delay(delayMs);

        var subscribers = _subscribers[type]
            .OrderByDescending(s => s.Priority)
            .ToList();

        var toRemove = new List<EventSubscriber>();

        foreach (var subscriber in subscribers)
        {
            if (subscriber.Condition != null && !subscriber.Condition(data))
                continue;

            var callback = (Action<T>)subscriber.Callback;

            if (subscriber.IsAsync)
            {
                _ = Task.Run(() =>
                {
                    try { callback.Invoke(data); }
                    catch (Exception ex) { Debug.LogError($"[PixEvent] Async Exception: {ex}"); }
                });
            }
            else
            {
                try { callback.Invoke(data); }
                catch (Exception ex) { Debug.LogError($"[PixEvent] Exception: {ex}"); }
            }

            if (subscriber.OneTime)
                toRemove.Add(subscriber);
        }

        foreach (var sub in toRemove)
            _subscribers[type].Remove(sub);
    }

    public class EmptyEvent : IEvent { }

    public static void Subscribe(Action callback, int priority = 0, bool async = false)
        => Subscribe<EmptyEvent>(_ => callback(), priority, async);

    public static void OneTimeSubscribe(Action callback, int priority = 0, bool async = false)
        => OneTimeSubscribe<EmptyEvent>(_ => callback(), priority, async);

    public static void ConditionalSubscribe(Func<bool> condition, Action callback, int priority = 0, bool async = false)
        => ConditionalSubscribe<EmptyEvent>(_ => condition(), _ => callback(), priority, async);

    public static void Unsubscribe(Action callback)
        => Unsubscribe<EmptyEvent>(_ => callback());

    public static void UnsubscribeAll()
        => UnsubscribeAll<EmptyEvent>();

    public static bool HasSubscribers()
        => HasSubscribers<EmptyEvent>();

    public static void Publish(int delayMs = 0)
        => Publish(new EmptyEvent(), delayMs);
}
