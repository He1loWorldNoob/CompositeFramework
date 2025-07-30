using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public static class CompositeStateFilter
{
    
    /*public static List<T> ApplyComponents<T>(CmsEntity target, CmsEntity source)
    {
        target.id = source.id;

        foreach (var newComp in source.components)
        {
            var type = newComp.GetType();
            if (type.GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                continue;

            target.components.RemoveAll(c => c.GetType() == type);
            target.components.Add(newComp);
        }
    }
    
    private */
    
    
    public static List<T> FilterIgnoreSerializeComponents<T>(CompositeEntityState<T> state) where T : ICompositeStatePice
    {
        return state.components
            .Where(c => c.GetType().GetCustomAttribute<CmsComponentIgnoreAttribute>() == null)
            .ToList();
    }
}
