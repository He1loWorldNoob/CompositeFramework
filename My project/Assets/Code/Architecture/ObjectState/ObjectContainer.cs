using System.Collections.Generic;

public class ObjectContainer
{
    public IEnumerable<ObjectState> Objects => _objects.Values;
    private readonly Dictionary<int, ObjectState> _objects = new();
    private int _nextId;
    
    public ObjectState Create(CmsId cmsId)
    {
        var model = cmsId.ToEntity();
        return Create(model);
    }
    public ObjectState Create(CmsEntity model)
    {
        var obj = new ObjectState(model, _nextId++);
        obj.Define<Awake>(); // Помечаем объект для инициализации
        _objects.Add(obj.Id, obj);
        return obj;
    }


    public bool Has(int id, out ObjectState state)
        => _objects.TryGetValue(id, out state);
    public bool Has(int id) => _objects.ContainsKey(id);
        
    public ObjectState Get(int id) => _objects[id];

    public void Remove(int objId)
    {
        _objects.Remove(objId);
    }

    public void Clear()
    {
        _nextId = 0;
        _objects.Clear();
    }
}