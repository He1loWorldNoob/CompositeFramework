using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomPropertyDrawer(typeof(CmsId), true)]
public class CmsIdDrawer : PropertyDrawer
{
    private string[] _cmsIds;
    private GUIContent[] _options;
    private const string SEARCH_PATH = ProjectPath.MainFolder + "Resources/CMS";


    private void LoadIds()
    {
        var all = CMS.GetAll<CmsEntity>();
        _cmsIds = all.Select(x => x.id).ToArray();
        _options = _cmsIds.Select(id => new GUIContent(id)).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_cmsIds == null)
            LoadIds();

        EditorGUI.BeginProperty(position, label, property);

        var cmsIdProp = property.FindPropertyRelative("cmsId");
        int selectedIndex = Mathf.Max(0, Array.IndexOf(_cmsIds, cmsIdProp.stringValue));

        // Разделяем область: Popup слева, кнопка справа
        var popupRect = new Rect(position.x, position.y, position.width - 30, position.height);
        var buttonRect = new Rect(position.x + position.width - 28, position.y, 28, position.height);

        int newIndex = EditorGUI.Popup(popupRect, label, selectedIndex, _options);
        cmsIdProp.stringValue = _cmsIds.Length > newIndex ? _cmsIds[newIndex] : "";

        if (GUI.Button(buttonRect, "🔍"))
        {
            TryOpenEntity(cmsIdProp.stringValue);
        }

        EditorGUI.EndProperty();
    }

    private void TryOpenEntity(string cmsId)
    {
        if (string.IsNullOrEmpty(cmsId)) return;

        string path = Path.Combine(SEARCH_PATH, cmsId + ".prefab").Replace("\\", "/");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab != null)
        {
            var window = ScriptableObject.CreateInstance<CMSEntityInspector>();
            window.titleContent = new GUIContent($"Inspector: {prefab.name}");
            window.SetSelectedPrefab(prefab);
            window.Show();
        }
        else
        {
            Debug.LogWarning($"CMS Prefab not found at path: {path}");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}



public abstract class FilteredCmsIdDrawer<TTag> : PropertyDrawer where TTag :class, ICmsComponentDefinition
{
    private string[] _cmsIds;
    private GUIContent[] _options;
    private const string SEARCH_PATH = ProjectPath.MainFolder + "Resources/CMS";

    private void LoadIds()
    {
        var all = CMS.GetAll<CmsEntity>();
        _cmsIds = all
            .Where(e => e.Is<TTag>())
            .Select(x => x.id)
            .ToArray();
        _options = _cmsIds.Select(id => new GUIContent(id)).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_cmsIds == null)
            LoadIds();

        EditorGUI.BeginProperty(position, label, property);

        var cmsIdProp = property.FindPropertyRelative("cmsId");
        int selectedIndex = Mathf.Max(0, Array.IndexOf(_cmsIds, cmsIdProp.stringValue));

        var popupRect = new Rect(position.x, position.y, position.width - 30, position.height);
        var buttonRect = new Rect(position.x + position.width - 28, position.y, 28, position.height);

        int newIndex = EditorGUI.Popup(popupRect, label, selectedIndex, _options);
        cmsIdProp.stringValue = _cmsIds.Length > newIndex ? _cmsIds[newIndex] : "";

        if (GUI.Button(buttonRect, "🔍"))
        {
            TryOpenEntity(cmsIdProp.stringValue);
        }

        EditorGUI.EndProperty();
    }

    private void TryOpenEntity(string cmsId)
    {
        if (string.IsNullOrEmpty(cmsId)) return;

        string path = Path.Combine(SEARCH_PATH, cmsId + ".prefab").Replace("\\", "/");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab != null)
        {
            var window = ScriptableObject.CreateInstance<CMSEntityInspector>();
            window.titleContent = new GUIContent($"Inspector: {prefab.name}");
            window.SetSelectedPrefab(prefab);
            window.Show();
        }
        else
        {
            Debug.LogWarning($"CMS Prefab not found at path: {path}");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
