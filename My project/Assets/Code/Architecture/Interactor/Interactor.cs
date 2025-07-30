using System;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractor
{
    void BindInteraction(BaseInteraction baseInteraction);
    void CallAll<T>(Action<T> action);
    List<T> FindAll<T>();
    void Clear();
}





public class Interactor : IInteractor
{
    private readonly List<BaseInteraction> _interactions = new();
    private readonly Dictionary<Type, object> _interactionsCache = new();

    public void BindInteraction(BaseInteraction baseInteraction)
    {
        _interactions.Add(baseInteraction);
    }
    

    public void CallAll<T>(Action<T> action)
    {
        FindAll<T>().ForEach(action);
    }


    public List<T> FindAll<T>()
    {
        if (_interactionsCache.TryGetValue(typeof(T), out var cached))
            return (List<T>)cached;
        var list = new List<T>();
        foreach (var i in _interactions)
            if (i is T t) list.Add(t);

        list.Sort((a, b) => ((BaseInteraction)(object)a).Priority() - ((BaseInteraction)(object)b).Priority());
        _interactionsCache[typeof(T)] = list; // no cast needed
        return list;
    }

    public void Clear()
    {
        _interactions.Clear();
        _interactionsCache.Clear();
    }
}