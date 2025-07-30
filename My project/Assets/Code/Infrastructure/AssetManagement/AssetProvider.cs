using System.Collections.Generic;
using UnityEngine;

public static class AssetProvider
{
    public static T Instantiate<T>(string path, Vector3 at) where T : Object
    {
        T prefab = Resources.Load<T>(path);
        return GameObject.Instantiate(prefab, at, Quaternion.identity);
    }
    public static T Instantiate<T>(string path, Transform parent) where T : Object
    {
        T prefab = Resources.Load<T>(path);
        return GameObject.Instantiate(prefab, parent);
    }
    public static T Instantiate<T>(string path, Vector3 at, Transform parent) where T : Object
    {
        T prefab = Resources.Load<T>(path);
        return GameObject.Instantiate(prefab, at, Quaternion.identity, parent);
    }
    public static T Instantiate<T>(string path) where T : Object
    {
        T prefab = Resources.Load<T>(path);
        return GameObject.Instantiate(prefab);
    }


    public static GameObject Instantiate(string path)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab);
    }

    public static T[] LoadAssets<T>(string path) where T : Object
    {
        T[] ressource = Resources.LoadAll<T>(path);
        return ressource;
    }
    public static T LoadAsset<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}