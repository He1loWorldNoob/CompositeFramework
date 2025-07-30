using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.CMSEditor.BuildModules
{
    public class CmsBuildProcessorModule: IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.WebGL)
            {
                Debug.Log("[CMS] Auto-filling CMS IDs before WebGL build...");
                CMSUtil.AutoFillCMSIds();
            }
        }
    }
}
[InitializeOnLoad]
public static class CMSEntityIdSetter
{
    
    static CMSEntityIdSetter()
    {
        EditorApplication.delayCall += AutoFillCMSIds;
    }
    [MenuItem("CMS/Auto-Fill IDs")]

    private static void AutoFillCMSIds()
    {
        CMSUtil.AutoFillCMSIds();

    }


    

}