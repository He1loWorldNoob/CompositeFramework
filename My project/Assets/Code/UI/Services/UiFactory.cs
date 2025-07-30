using System;
using UnityEngine;

public class UiFactory
{
    private readonly Scene _scene;
    private Transform _uiRoot;
    public UiFactory(Scene scene)
    {
        _scene = scene;
    }
    public T CreateWindow<T>(WindowId id) where T : UIWindowBase
    {
        CheckRoot();
        return CreateWindow<T>(id.ToEntity());
    }

    public T CreateWindow<T>(CmsEntity entity) where T : UIWindowBase
    {
        if (!entity.Is<UiWindowTag>(out var tag))
            throw new Exception("Not a UiWindowTag");
        return _scene.DiContainer.InstantiatePrefab(tag.windowPrefab, _uiRoot).GetComponent<T>();
    }
    
    public void CreateHud(string hudPath)
    {
        CheckRoot();
        var prefab = AssetProvider.LoadAsset<GameObject>(hudPath);
        _scene.DiContainer.InstantiatePrefab(prefab,_uiRoot);
    }
    
    private void CheckRoot()
    {
        if (_uiRoot != null) return;
        _uiRoot = AssetProvider.Instantiate<GameObject>(UiPath.UIRoot).transform;

    }
}