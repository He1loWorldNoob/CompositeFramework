using System;
using System.Collections.Generic;

public class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();

        _subscribers[type].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var list))
        {
            list.Remove(handler);
            if (list.Count == 0)
                _subscribers.Remove(type);
        }
    }

    public void Fire<T>(T evt)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var list)) return;
        foreach (var handler in list)
            ((Action<T>)handler).Invoke(evt);
    }

    public void Clear()
    {
        _subscribers.Clear();
    }
}