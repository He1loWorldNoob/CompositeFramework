using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;


public interface ICompositeStatePice { }

[Serializable]
public class CompositeEntityState<TComponent> where TComponent : ICompositeStatePice
{
    [SerializeReference, SubclassSelector]
    public List<TComponent> components = new();
    
    [JsonIgnore]
    public readonly Dictionary<Type, TComponent> ComponentCache = new();

    public T Get<T>() where T : class, TComponent
    {
        if (ComponentCache.TryGetValue(typeof(T), out var cachedComponent))
            return cachedComponent as T;

        foreach (var c in components)
        {
            if (c is T ct)
            {
                ComponentCache[typeof(T)] = ct; // Кэшируем найденный компонент
                return ct;
            }
        }

        return null;
    }
    
    
    public List<T> GetAll<T>() 
    {
        List<T> list = new();
        foreach(var c in components)
            if (c is T ct) list.Add(ct);
        return list;
    }

    public void Add<T>(T component) where T : class, TComponent
    {
        if (Is<T>())
        {
            Remove<T>();   
        }
        ComponentCache[typeof(T)] = component;
        components.Add(component);

    }
    
    public T Define<T>() where T : class, TComponent, new()
    {
        var t = Get<T>();
        if (t != null)
            return t;

        var entityComponent = new T();
        components.Add(entityComponent);
        ComponentCache[typeof(T)] = entityComponent; // Добавляем в кэш
        return entityComponent;
    }


    public void Remove<T>() where T : TComponent
    {
        if (ComponentCache.TryGetValue(typeof(T), out var cachedComponent))
        {
            ComponentCache.Remove(typeof(T)); // Удаляем из кэша
        }
        components.Remove(cachedComponent);
    }

    public bool Is<T>() where T : class, TComponent
    {
        return Get<T>() != null;
    }
    public bool Is<T>(out T t) where T : class, TComponent
    {
        t = Get<T>();
        return t != null;
    }
    public bool Is(Type type)
    {
        return components.Find(m => m.GetType() == type) != null;
    }

    public void Clear()
    {
        ComponentCache.Clear();
        components.Clear();
    }
}