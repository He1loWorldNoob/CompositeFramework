using System;
using System.Collections.Generic;
using Code.Infrastructure.CMSSystem;
using UnityEngine;

public static class CMS
{
    static CMSTable<CmsEntity> all = new CMSTable<CmsEntity>();
    
    static bool isInit;
    
    public static void Init()
    {
        if (isInit)
            return;
        isInit = true;

#if UNITY_EDITOR
        CMSUtil.AutoFillCMSIds();
#endif        
        AutoAdd();
    }
    
    static void AutoAdd()
    {
        var subs = ReflectionUtil.FindAllSubslasses<CmsEntity>();
        foreach (var subclass in subs)
            all.Add(Activator.CreateInstance(subclass) as CmsEntity);
        var resources = Resources.LoadAll<CMSEntityPfb>("CMS");
        foreach (var resEntity in resources)
        {
            Debug.Log("LOAD ENTITY " + resEntity.entityState.id);
            var entity= new CmsEntity()
            {
                id = resEntity.entityState.id,
                components = resEntity.entityState.components,
            };
            all.Add(entity);
        }
    }

    public static T Get<T>(string def_id = null) where T : CmsEntity
    {
        if (def_id == null)
            def_id = E.Id<T>();
        var findById = all.FindById(def_id) as T;

        if (findById == null)
        {
            // ok fuck it
            throw new Exception("unable to resolve entity id '" + def_id + "'");
        }

        return findById;
    }


    public static T GetData<T>(string def_id = null) where T : class, ICmsComponentDefinition
    {
        return Get<CmsEntity>(def_id).Get<T>();
    }

    public static List<T> GetAll<T>() where T : CmsEntity
    {
        var allSearch = new List<T>();

        foreach (var a in all.GetAll())
            if (a is T)
                allSearch.Add(a as T);

        return allSearch;
    }
    

    public static List<(CmsEntity e, T tag)> GetAllWithTag<T>() where T : class, ICmsComponentDefinition, new()
    {
        var allSearch = new List<(CmsEntity, T)>();

        foreach (var a in all.GetAll())
            if (a.Is<T>(out var t))
                allSearch.Add((a, t));

        return allSearch;
    }

    public static void Unload()
    {
        isInit = false;
        all = new CMSTable<CmsEntity>();
    }
}

public class CMSTable<T> where T : CmsEntity, new()
{
    public List<T> list = new List<T>();
    Dictionary<string, T> dict = new Dictionary<string, T>();

    public void Add(T inst)
    {
        if (inst.id == null)
            inst.id = E.Id(inst.GetType());
        
        list.Add(inst);
        dict.Add(inst.id, inst);
    }
    
    public T New(string id)
    {
        var t = new T();
        t.id = id;
        list.Add(t);
        dict.Add(id, t);
        return t;
    }
    public List<T> GetAll()
    {
        return list;
    }

    public T FindById(string id)
    {
        return dict.GetValueOrDefault(id);
    }

    public T2 FindByType<T2>() where T2 : T
    {
        foreach(var v in list)
            if (v is T2 v2)
                return v2;
        return null;
    }
}
public static class E
{
    public static string Id(Type getType)
    {
        return getType.FullName;
    }
    
    public static string Id<T>()
    {
        return ID<T>.Get();
    }
}


static class ID<T>
{
    static string cache;
    
    public static string Get()
    {
        if (cache == null)
            cache = typeof(T).FullName;
        return cache;
    }
    
    public static string Get<T>()
    {
        return ID<T>.Get();
    }
}