using System;
using UnityEngine;

public abstract class ViewComponent : MonoBehaviour
{
    private ObjectState _state;
    public ObjectState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateInitialized();
        }
    }

    protected virtual void OnStateInitialized(){}
    
}


public static class ViewComponentExtensions
{
    public static bool IsView<T>(this ObjectState state, out T component) where T : ViewComponent
    {
        component = null;
        return state.Is<ObjectViewComponent>(out var view) && view.View.Is(out component);
    }
}