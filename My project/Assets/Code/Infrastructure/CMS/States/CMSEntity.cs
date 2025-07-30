using System;

[Serializable]
public class CmsEntity : CompositeEntityState<ICmsComponentDefinition>
{
    
    public string id;
}
public interface ICmsComponentDefinition : ICompositeStatePice { }
