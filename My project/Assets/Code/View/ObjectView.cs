using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectViewComponent : IObjectComponent
{
    public ObjectView View;
}

public class ObjectView : MonoBehaviour
{
    [SerializeField]
    private ObjectState state;

    public ObjectState State
    {
        get => state;
        set
        {

            state = value;
            gameObject.name += $" ({state.Id})";
            foreach (var component in components)
                component.State = state;
        }
    }
    
    
    public ViewComponent[] components;
    private Dictionary<Type, ViewComponent> _cache = new();


    private void Awake()
    {
        components = GetComponentsInChildren<ViewComponent>();
    }


    public T Get<T>() where T : ViewComponent
    {
        if (_cache.ContainsKey(typeof(T)))
            return _cache[typeof(T)] as T;
        foreach (var component in components)
        {
            if (component is T viewComponent)
            {
                _cache.Add(typeof(T), viewComponent);
                return viewComponent;
            }
        }

        return null;
    }

    public bool Is<T>(out T component) where T : ViewComponent
    {
        component = Get<T>();
        return component != null;
    }
}
