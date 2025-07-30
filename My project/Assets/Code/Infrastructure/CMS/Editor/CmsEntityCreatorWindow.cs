using UnityEditor;
using UnityEngine;
using Code.Infrastructure.CMSSystem;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class CmsEntityCreatorWindow : EditorWindow
{
    private string jsonInput = "";
    private TextAsset jsonFile;

    private string previewPath = "";
    private string previewName = "";
    private const string CmsPath = ProjectPath.CMSFolder;

    private Vector2 scrollPos;
    private Vector2 jsonScroll;

    [MenuItem("Tools/CMS/Create CMSEntityPfb from JSON")]
    public static void ShowWindow()
    {
        GetWindow<CmsEntityCreatorWindow>("CMS Entity Creator");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("📄 JSON File (TextAsset)", EditorStyles.boldLabel);
        var newJsonFile = (TextAsset)EditorGUILayout.ObjectField("File", jsonFile, typeof(TextAsset), false);
        if (newJsonFile != jsonFile)
        {
            jsonFile = newJsonFile;
            jsonInput = ""; // Сброс ручного текста
            UpdatePreviewFromJson(jsonFile?.text);
        }

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("✍️ Or paste JSON manually", EditorStyles.boldLabel);
        jsonScroll = EditorGUILayout.BeginScrollView(jsonScroll, GUILayout.Height(150));
        string newJsonInput = EditorGUILayout.TextArea(jsonInput, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (newJsonInput != jsonInput)
        {
            jsonInput = newJsonInput;
            jsonFile = null; // Сброс файла
            UpdatePreviewFromJson(jsonInput);
        }

        EditorGUILayout.Space(10);

        if (!string.IsNullOrEmpty(previewPath))
        {
            GUIStyle previewStyle = new GUIStyle(EditorStyles.label);
            previewStyle.normal.textColor = Color.cyan;

            EditorGUILayout.LabelField("📂 Target Path:", $"{CmsPath}{previewPath}", previewStyle);
            EditorGUILayout.LabelField("📦 Prefab Name:", previewName, previewStyle);

            string fullPath = Path.Combine(CmsPath, previewPath, $"{previewName}.prefab");
            if (File.Exists(fullPath))
            {
                GUIStyle warnStyle = new GUIStyle(EditorStyles.label);
                warnStyle.normal.textColor = Color.yellow;
                EditorGUILayout.LabelField("⚠ Префаб уже существует и будет перезаписан!", warnStyle);
            }
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("✅ Create Entity Prefab"))
        {
            try
            {
                string json = jsonFile != null ? jsonFile.text : jsonInput;
                if (string.IsNullOrWhiteSpace(json))
                    throw new Exception("JSON input is empty.");

                CreatePrefabFromJson(json);
                Debug.Log("✅ Prefab created successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Failed to create prefab: " + ex.Message);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void UpdatePreviewFromJson(string json)
    {
        previewPath = "";
        previewName = "";

        if (string.IsNullOrWhiteSpace(json)) return;

        try
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            CmsEntity entity = JsonConvert.DeserializeObject<CmsEntity>(json, settings);

            if (!string.IsNullOrWhiteSpace(entity?.id))
            {
                string[] parts = entity.id.Split('/');
                if (parts.Length >= 2)
                {
                    previewPath = Path.Combine(parts[..^1]);
                    previewName = parts[^1];
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("⚠ Не удалось распарсить id из json: " + ex.Message);
        }
    }

private void CreatePrefabFromJson(string json)
{
    var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
    CmsEntity newEntity = JsonConvert.DeserializeObject<CmsEntity>(json, settings);
    if (newEntity == null)
        throw new Exception("CmsEntity десериализация вернула null.");
    if (string.IsNullOrWhiteSpace(newEntity.id))
        throw new Exception("CmsEntity должен содержать поле id.");

    string[] idParts = newEntity.id.Split('/');
    if (idParts.Length < 2)
        throw new Exception("Неверный формат id: " + newEntity.id);

    string pathFolder = Path.Combine(CmsPath, Path.Combine(idParts[..^1]));
    string prefabName = idParts[^1];
    string prefabPath = Path.Combine(pathFolder, $"{prefabName}.prefab").Replace("\\", "/");

    Directory.CreateDirectory(pathFolder);

    GameObject go;
    CMSEntityPfb component;

    if (File.Exists(prefabPath))
    {
        // Загрузка и обновление существующего префаба
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        go = (GameObject)PrefabUtility.InstantiatePrefab(existing);
        component = go.GetComponent<CMSEntityPfb>() ?? go.AddComponent<CMSEntityPfb>();

        if (component.entityState == null)
            component.entityState = new CmsEntity();

        var target = component.entityState;
        target.id = newEntity.id;
        
        foreach (var newComp in newEntity.components)
        {
            var type = newComp.GetType();
            if (type.GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                continue;

            target.components.RemoveAll(c => c.GetType() == type);

            target.components.Add(newComp); // Добавить новый (или перезаписанный)
        }
    }
    else
    {
        // Создание нового
        go = new GameObject(prefabName);
        component = go.AddComponent<CMSEntityPfb>();
        component.entityState = newEntity;
    }

    PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
    DestroyImmediate(go);
    AssetDatabase.Refresh();

    Debug.Log($"📁 Saved prefab to: {prefabPath}");
}

}
