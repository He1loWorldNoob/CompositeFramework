using System;

public class Signal<T>
{
    private event Action<T> _listeners;

    public void Subscribe(Action<T> listener) => _listeners += listener;
    public void Unsubscribe(Action<T> listener) => _listeners -= listener;
    public void Fire(T value) => _listeners?.Invoke(value);
}
public class Signal
{
    private event Action _listeners;

    public void Subscribe(Action listener) => _listeners += listener;
    public void Unsubscribe(Action listener) => _listeners -= listener;
    public void Fire() => _listeners?.Invoke();
}
