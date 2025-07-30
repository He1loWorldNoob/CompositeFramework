using Code.Infrastructure.CMSSystem;
using UnityEngine;

public static class CmsEntitLoader
{
    public static CmsEntity LoadCmsEntityPrefab(this string entityPath)
    {
        return Resources.Load<CMSEntityPfb>(entityPath).entityState;
    }
    public static T GetCmsEntity<T>(this string entityPath) where T : CmsEntity
    {
        return CMS.Get<T>(entityPath);
    }
    public static CmsEntity GetCmsEntity(this string entityPath)
    {
        return CMS.Get<CmsEntity>(entityPath);
    }
    
}