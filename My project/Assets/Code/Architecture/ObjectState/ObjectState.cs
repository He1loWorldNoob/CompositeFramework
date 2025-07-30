using System;
using UnityEngine;

public interface IObjectComponent : ICompositeStatePice { }


[Serializable]
public class ObjectState : CompositeEntityState<IObjectComponent>
{
    [SerializeField] private CmsEntity model;
    [SerializeField] private int id;
    
    public int Id => id;
    public CmsEntity Model => model;
    
    public ObjectState(CmsEntity model, int id)
    {
        this.model = model;
        this.id = id;
    }
    
    public override string ToString()
    {
        return Model.id;
    }
    
}



[Serializable]
public class TransformComponent : IObjectComponent
{
    public Vector3 position;
    public Vector3 scale = Vector3.one;
}
