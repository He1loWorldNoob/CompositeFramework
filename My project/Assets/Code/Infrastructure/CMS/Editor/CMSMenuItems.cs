using UnityEditor;
using UnityEngine;

public static class CMSMenuItems
{
    [MenuItem("CMS/Reload")]
    public static void CMSReload()
    {
        CMS.Unload();
        CMS.Init();
    }
    
    [MenuItem("CMS/Generate")]
    public static void Generate()
    {
        ResourcePathGenerator.PathGenerate();
        //CmsEnumGenerator.Generate();
    }
}