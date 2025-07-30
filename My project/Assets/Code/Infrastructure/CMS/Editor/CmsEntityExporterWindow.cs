using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Code.Infrastructure.CMSSystem;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class CmsEntityExporterWindow : EditorWindow
{
    private GameObject selectedPrefab;
    private string jsonOutput = "";
    private string exportPath = ProjectPath.MainFolder + "ExportedCmsEntities";
    private string fileName = "";
    private Vector2 scrollPos;
    private Vector2 jsonScroll;

    [MenuItem("Tools/CMS/Export CMSEntityPfb to JSON")]
    public static void ShowWindow()
    {
        GetWindow<CmsEntityExporterWindow>("CMS Entity Exporter");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("📦 Select CMSEntityPfb GameObject", EditorStyles.boldLabel);
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab or Scene Object", selectedPrefab, typeof(GameObject), true);

        if (selectedPrefab != null)
        {
            CMSEntityPfb cms = selectedPrefab.GetComponent<CMSEntityPfb>();
            if (cms == null)
            {
                EditorGUILayout.HelpBox("Выбранный объект не содержит компонент CMSEntityPfb.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            if (GUILayout.Button("🔄 Convert to JSON"))
            {
                try
                {
                    var components = new List<ICmsComponentDefinition>();
                    foreach (var component in cms.entityState.components)
                    {
                        if (component.GetType().GetCustomAttribute<CmsComponentIgnoreAttribute>() == null)
                        {
                            components.Add(component);
                        }
                    }
                    var original = cms.entityState;
                    var newEntity = new CmsEntity()
                    {
                        id = original.id,
                        components = components,
                    };

                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Formatting = Formatting.Indented,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                    };

                    jsonOutput = JsonConvert.SerializeObject(newEntity, settings);
                    fileName = newEntity.id?.Replace("/", "_") ?? "UnnamedEntity";

                    Debug.Log("✅ JSON успешно сгенерирован.");
                }
                catch (Exception ex)
                {
                    Debug.LogError("❌ Ошибка сериализации: " + ex.Message);
                }
            }

            EditorGUILayout.Space(10);
        }

        if (!string.IsNullOrEmpty(jsonOutput))
        {
            EditorGUILayout.LabelField("📝 JSON Preview", EditorStyles.boldLabel);
            jsonScroll = EditorGUILayout.BeginScrollView(jsonScroll, GUILayout.Height(250));
            jsonOutput = EditorGUILayout.TextArea(jsonOutput, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("💾 Save Settings", EditorStyles.boldLabel);
            exportPath = EditorGUILayout.TextField("Export Folder", exportPath);
            fileName = EditorGUILayout.TextField("File Name", fileName);

            if (GUILayout.Button("💾 Save to File"))
            {
                try
                {
                    Directory.CreateDirectory(exportPath);
                    string path = Path.Combine(exportPath, $"{fileName}.json");
                    File.WriteAllText(path, jsonOutput);
                    AssetDatabase.Refresh();
                    Debug.Log($"📁 JSON saved to: {path}");
                }
                catch (Exception ex)
                {
                    Debug.LogError("❌ Failed to save JSON: " + ex.Message);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
