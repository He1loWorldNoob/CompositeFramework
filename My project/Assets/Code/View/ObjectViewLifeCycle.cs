using System;
using UnityEngine;


[Serializable]
public class TagPrefab : ICmsComponentDefinition
{
    public ObjectView prefab;
}

public class ObjectViewLifeCycle : IStartObjectModule, IDestroyObjectModule
{
    private readonly Scene _scene;

    public ObjectViewLifeCycle(Scene scene)
    {
        _scene = scene;
    }

    public void OnStartObject(ObjectState state)
    {
        if (!state.Model.Is<TagPrefab>(out var prefab)) return;
        
        var view = GameObject.Instantiate(prefab.prefab);
        view.State = state;
        foreach (var component in view.components)
            _scene.DiContainer.Inject(component);
        
        state.Define<ObjectViewComponent>().View = view;
    }

    public void OnDestroyObject(ObjectState state)
    {
        if(!state.Is<ObjectViewComponent>(out var objectView)) return;
        GameObject.Destroy(objectView.View.gameObject);
    }
}