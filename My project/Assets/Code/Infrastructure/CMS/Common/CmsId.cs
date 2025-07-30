using System;

[Serializable] public class AbilityId : CmsId { }
[Serializable] public class EquipId : CmsId { }

[Serializable] public class ProjectileId : CmsId { }
[Serializable] public class EffectId : CmsId { }
[Serializable] public class ClassId : CmsId { }
[Serializable] public class WindowId : CmsId { }


[Serializable]
public class CmsId
{
    public string cmsId;

    public CmsEntity ToEntity() => 
        cmsId.GetCmsEntity();
}

