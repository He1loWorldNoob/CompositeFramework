using Code.Infrastructure.CMSSystem;
using UnityEditor;
using UnityEngine;

public static class CMSUtil
{
    public static T Load<T>(this string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    
    public static Sprite LoadFromSpritesheet(string imageName, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>(imageName);
 
        foreach(var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }
    
    const string resorcePath = ProjectPath.MainFolder + "Resources/CMS/";

    public static void AutoFillCMSIds()
    {
        var guids = AssetDatabase.FindAssets("t:GameObject", new[] { resorcePath });
        var updatedCount = 0;
        //Debug.Log(guids.Length);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            var entity = prefab.GetComponent<CMSEntityPfb>();
            if (entity == null || entity.entityState == null) continue;

            // Принудительно получить корректный ID
            var newId = BuildIdFromPath(path);
            //Debug.Log(entity.entityState.id + " " + newId);
            // Проверим, нужно ли обновлять
            if (entity.entityState.id != newId)
            {
                entity.entityState.id = newId;

                // Отметить объект как изменённый
                EditorUtility.SetDirty(entity);
                PrefabUtility.RecordPrefabInstancePropertyModifications(entity);
                updatedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[CMS] Updated {updatedCount} prefab ID(s).");
    }
    public static string BuildIdFromPath(string fullAssetPath)
    {
        const string extension = ".prefab";

        if (!fullAssetPath.StartsWith(resorcePath) || !fullAssetPath.EndsWith(extension))
            return null;

        string trimmedPath = fullAssetPath.Substring(resorcePath.Length); // убираем Assets/Resources/CMS/
        trimmedPath = trimmedPath.Substring(0, trimmedPath.Length - extension.Length); // убираем .prefab

        return trimmedPath; // оставляем вложенную структуру как есть: Ability/ChainBow
    }
    
    
    
}

public static class CmsEntityExtensions
{
    public static Sprite GetSprite(this CmsEntity model)
    {
        if (model.Is(out TagSprite sprite))
        {
            return sprite.sprite;
        }
        return null;
    }
}