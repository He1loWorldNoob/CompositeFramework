using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Infrastructure.CMSSystem;
using Newtonsoft.Json;
using System;
using System.Reflection;

public class CMSEntityExplorer : EditorWindow
{
    private const string SEARCH_PATH = ProjectPath.MainFolder + "Resources/CMS";

    private string searchQuery = "";
    private Vector2 scrollPos;
    private string currentPath = SEARCH_PATH;
    private Dictionary<string, bool> folderFoldouts = new();
    private List<string> folderStack = new();
    private List<string> searchResults = new();
    private static GameObject selectedPrefab;
    private static CMSEntityInspector inspectorWindow;
    
    // Multi-file export/import settings
    private string exportPath = ProjectPath.MainFolder + "ExportedCmsEntities";
    private string importPath = ProjectPath.MainFolder + "ExportedCmsEntities";
    private bool showExportImportPanel = false;

    [MenuItem("CMS/Entity Explorer")]
    public static void ShowWindow()
    {
        var window = GetWindow<CMSEntityExplorer>();
        window.titleContent = new GUIContent("CMS Entity Explorer");
        window.Show();
    }

    private void OnEnable()
    {
        CMS.Init();
        PerformSearch();
    }

    private void PerformSearch()
    {
        searchResults.Clear();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            var allFiles = Directory.GetFiles(currentPath, "*.prefab", SearchOption.AllDirectories);
            searchResults = allFiles
                .Where(f => Path.GetFileNameWithoutExtension(f).ToLower().Contains(searchQuery.ToLower()))
                .ToList();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (folderStack.Count > 0 && GUILayout.Button("← Назад", GUILayout.Width(80)))
        {
            GoBack();
        }

        string newSearchQuery = EditorGUILayout.TextField("Search:", searchQuery);
        if (newSearchQuery != searchQuery)
        {
            searchQuery = newSearchQuery;
            PerformSearch();
        }

        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            PerformSearch();
        }
        
        if (GUILayout.Button("📤📥", GUILayout.Width(40)))
        {
            showExportImportPanel = !showExportImportPanel;
        }
        EditorGUILayout.EndHorizontal();
        
        // Export/Import Panel
        if (showExportImportPanel)
        {
            DrawExportImportPanel();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (!string.IsNullOrEmpty(searchQuery))
        {
            DisplaySearchResults();
        }
        else
        {
            DisplayFolderContents(currentPath);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DisplaySearchResults()
    {
        if (searchResults.Count == 0)
        {
            EditorGUILayout.HelpBox("No results found.", MessageType.Info);
            return;
        }

        foreach (var file in searchResults)
        {
            DisplayPrefabEntry(file);
        }
    }

    private void DisplayFolderContents(string path)
    {
        if (!Directory.Exists(path)) return;

        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path, "*.prefab");

        var validDirectories = directories.Where(dir =>
            Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories).Length > 0
            || Directory.GetDirectories(dir, "*", SearchOption.AllDirectories).Length > 0
        ).ToList();

        foreach (var directory in validDirectories)
        {
            string folderName = Path.GetFileName(directory);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("📁", GUILayout.Width(20));
            if (GUILayout.Button(folderName, EditorStyles.label))
            {
                EnterFolder(directory);
            }
            EditorGUILayout.EndHorizontal();
        }

        foreach (var file in files)
        {
            DisplayPrefabEntry(file);
        }
    }

    private void DisplayPrefabEntry(string file)
    {
        string fileName = Path.GetFileNameWithoutExtension(file);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);

        if (prefab != null)
        {
            EditorGUILayout.BeginHorizontal();

            var cmsEntity = prefab.GetComponent<CMSEntityPfb>();
            if (cmsEntity != null)
            {
                Sprite sprite = cmsEntity.entityState.GetSprite();
                if (sprite != null)
                {
                    Rect spriteRect = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(50), GUILayout.Height(50));
                    GUIDrawSprite(spriteRect, sprite);
                }
                else
                {
                    Texture2D icon = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
                    GUILayout.Label(icon, GUILayout.Width(50), GUILayout.Height(50));
                }
            }
            else
            {
                GUILayout.Label("[No Icon]", GUILayout.Width(50), GUILayout.Height(50));
            }

            GUILayout.Label(TruncateString(fileName, 30), EditorStyles.label, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("▶", GUILayout.Width(30), GUILayout.Height(30)))
            {
                OpenInspectorWindow(prefab);
                SelectPrefab(prefab, file);
            }
            if (GUILayout.Button("NewWindow", GUILayout.Width(30), GUILayout.Height(30)))
            {
                OpenNewInspectorWindow(prefab);
                SelectPrefab(prefab, file);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void SelectPrefab(GameObject prefab, string assetPath)
    {
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }

    private void OpenInspectorWindow(GameObject prefab)
    {
        selectedPrefab = prefab;

        if (inspectorWindow == null)
        {
            inspectorWindow = GetWindow<CMSEntityInspector>();
            inspectorWindow.titleContent = new GUIContent("CMS Prefab Inspector");
        }

        inspectorWindow.SetSelectedPrefab(prefab);
        inspectorWindow.Show();
    }

    private void OpenNewInspectorWindow(GameObject prefab)
    {
        var inspectorWindow = CreateInstance<CMSEntityInspector>();
        inspectorWindow.titleContent = new GUIContent($"Inspector: {prefab.name}");
        inspectorWindow.SetSelectedPrefab(prefab);
        inspectorWindow.Show();
    }

    private string TruncateString(string text, int maxLength)
    {
        if (text.Length > maxLength)
        {
            return text.Substring(0, maxLength - 3) + "...";
        }
        return text;
    }

    public static void GUIDrawSprite(Rect rect, Sprite sprite)
    {
        if (sprite == null || sprite.texture == null) return;

        Rect spriteRect = sprite.rect;
        Texture2D tex = sprite.texture;
        Rect texCoords = new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height);

        GUI.DrawTextureWithTexCoords(rect, tex, texCoords);
    }

    private void EnterFolder(string path)
    {
        folderStack.Add(currentPath);
        currentPath = path;
        PerformSearch();
    }

    private void GoBack()
    {
        if (folderStack.Count > 0)
        {
            currentPath = folderStack.Last();
            folderStack.RemoveAt(folderStack.Count - 1);
            PerformSearch();
        }
    }
    
    private void DrawExportImportPanel()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("📤📥 Multi-File Export/Import", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Export Section
        EditorGUILayout.LabelField("📤 Export", EditorStyles.boldLabel);
        exportPath = EditorGUILayout.TextField("Export Path:", exportPath);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📤 Export All Entities"))
        {
            ExportAllEntities();
        }
        if (GUILayout.Button("📤 Export Current Folder"))
        {
            ExportCurrentFolder();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        // Import Section
        EditorGUILayout.LabelField("📥 Import", EditorStyles.boldLabel);
        importPath = EditorGUILayout.TextField("Import Path:", importPath);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📥 Import All JSONs"))
        {
            ImportAllEntities();
        }
        if (GUILayout.Button("🔍 Browse Import Folder"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Import Folder", importPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                importPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
    
    private void ExportAllEntities()
    {
        try
        {
            CMS.Init();
            var allEntities = CMS.GetAll<CmsEntity>();

            if (allEntities == null || allEntities.Count == 0)
            {
                EditorUtility.DisplayDialog("Export Error", "No entities found to export.", "OK");
                return;
            }

            int exported = 0;
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            foreach (var entity in allEntities)
            {
                if (string.IsNullOrWhiteSpace(entity.id)) continue;

                // Фильтрация компонентов
                var filteredComponents = new List<ICmsComponentDefinition>();
                foreach (var component in entity.components)
                {
                    if (component == null) continue;
                    if (component.GetType().GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                        continue;

                    filteredComponents.Add(component);
                }

                var exportEntity = new CmsEntity
                {
                    id = entity.id,
                    components = filteredComponents
                };

                // Построение структуры путей по id
                string[] idParts = entity.id.Split('/');
                if (idParts.Length < 2) continue;

                string subfolderPath = Path.Combine(exportPath, Path.Combine(idParts[..^1]));
                Directory.CreateDirectory(subfolderPath);

                string fileName = idParts[^1] + ".json";
                string filePath = Path.Combine(subfolderPath, fileName);

                string json = JsonConvert.SerializeObject(exportEntity, settings);
                File.WriteAllText(filePath, json);
                exported++;

                EditorUtility.DisplayProgressBar("Exporting Entities", $"Exported: {entity.id}", (float)exported / allEntities.Count);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Export Complete",
                $"Successfully exported {exported} entities to:\n{exportPath}", "OK");
            Debug.Log($"✅ Exported {exported} entities to: {exportPath}");
        }
        catch (Exception ex)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Export Error", $"Failed to export entities:\n{ex.Message}", "OK");
            Debug.LogError($"❌ Export failed: {ex.Message}");
        }
    }


    
    private void ExportCurrentFolder()
{
    try
    {
        if (!Directory.Exists(currentPath))
        {
            EditorUtility.DisplayDialog("Export Error", "Current folder does not exist.", "OK");
            return;
        }

        var prefabFiles = Directory.GetFiles(currentPath, "*.prefab", SearchOption.AllDirectories);
        if (prefabFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("Export Error", "No prefab files found in current folder.", "OK");
            return;
        }

        int exported = 0;
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        };

        foreach (var prefabFile in prefabFiles)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFile);
            if (prefab == null) continue;

            var cmsEntity = prefab.GetComponent<CMSEntityPfb>();
            if (cmsEntity?.entityState == null) continue;

            var entity = cmsEntity.entityState;
            if (string.IsNullOrWhiteSpace(entity.id)) continue;

            // Фильтрация компонентов
            var filteredComponents = new List<ICmsComponentDefinition>();
            foreach (var component in entity.components)
            {
                if (component == null) continue;
                if (component.GetType().GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                    continue;

                filteredComponents.Add(component);
            }

            var exportEntity = new CmsEntity
            {
                id = entity.id,
                components = filteredComponents
            };

            // Структура папок по id
            string[] idParts = entity.id.Split('/');
            if (idParts.Length < 2) continue;

            string subfolderPath = Path.Combine(exportPath, Path.Combine(idParts[..^1]));
            Directory.CreateDirectory(subfolderPath);

            string fileName = idParts[^1] + ".json";
            string filePath = Path.Combine(subfolderPath, fileName);

            string json = JsonConvert.SerializeObject(exportEntity, settings);
            File.WriteAllText(filePath, json);
            exported++;

            EditorUtility.DisplayProgressBar("Exporting Folder", $"Exported: {entity.id}", 
                (float)exported / prefabFiles.Length);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Export Complete",
            $"Successfully exported {exported} entities from current folder to:\n{exportPath}", "OK");
        Debug.Log($"✅ Exported {exported} entities from current folder to: {exportPath}");
    }
    catch (Exception ex)
    {
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Export Error", $"Failed to export current folder:\n{ex.Message}", "OK");
        Debug.LogError($"❌ Export current folder failed: {ex.Message}");
    }
}


    
    private void ImportAllEntities()
    {
        try
        {
            if (!Directory.Exists(importPath))
            {
                EditorUtility.DisplayDialog("Import Error", "Import folder does not exist.", "OK");
                return;
            }

            var jsonFiles = Directory.GetFiles(importPath, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Import Error", "No JSON files found in import folder.", "OK");
                return;
            }

            int imported = 0;
            int errors = 0;
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    string json = File.ReadAllText(jsonFile);
                    var entity = JsonConvert.DeserializeObject<CmsEntity>(json, settings);

                    if (entity == null || string.IsNullOrWhiteSpace(entity.id))
                    {
                        Debug.LogWarning($"⚠ Skipped invalid entity in file: {jsonFile}");
                        errors++;
                        continue;
                    }

                    string[] idParts = entity.id.Split('/');
                    if (idParts.Length < 2)
                        throw new Exception($"Invalid ID format: {entity.id}");

                    string pathFolder = Path.Combine(ProjectPath.CMSFolder, Path.Combine(idParts[..^1]));
                    string prefabName = idParts[^1];
                    string prefabPath = Path.Combine(pathFolder, $"{prefabName}.prefab").Replace("\\", "/");

                    Directory.CreateDirectory(pathFolder);

                    GameObject go;
                    CMSEntityPfb component;

                    if (File.Exists(prefabPath))
                    {
                        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        go = (GameObject)PrefabUtility.InstantiatePrefab(existing);
                        component = go.GetComponent<CMSEntityPfb>() ?? go.AddComponent<CMSEntityPfb>();

                        if (component.entityState == null)
                            component.entityState = new CmsEntity();

                        component.entityState.id = entity.id;
                        component.entityState.components.RemoveAll(x=>x == null);
                        foreach (var newComp in entity.components)
                        {
                            var type = newComp.GetType();
                            if (type.GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                                continue;

                            component.entityState.components.RemoveAll(c => c.GetType() == type);
                            component.entityState.components.Add(newComp);
                        }
                    }
                    else
                    {
                        go = new GameObject(prefabName);
                        component = go.AddComponent<CMSEntityPfb>();
                        component.entityState = entity;
                    }

                    PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                    UnityEngine.Object.DestroyImmediate(go);

                    imported++;

                    EditorUtility.DisplayProgressBar("Importing Entities", $"Imported: {entity.id}", (float)imported / jsonFiles.Length);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to import {jsonFile}: {ex.Message}");
                    errors++;
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            CMS.Unload();
            CMS.Init();
            PerformSearch();

            string message = $"Import completed!\nImported: {imported}\nErrors: {errors}";
            EditorUtility.DisplayDialog("Import Complete", message, "OK");
            Debug.Log($"✅ Import completed: {imported} imported, {errors} errors");
        }
        catch (Exception ex)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Import Error", $"Failed to import entities:\n{ex.Message}", "OK");
            Debug.LogError($"❌ Import failed: {ex.Message}");
        }
    }

    
    private void CreatePrefabFromEntity(CmsEntity entity)
    {
        string[] idParts = entity.id.Split('/');
        if (idParts.Length < 2)
            throw new Exception($"Invalid entity ID format: {entity.id}");
        
        string pathFolder = Path.Combine(SEARCH_PATH, Path.Combine(idParts[..^1]));
        string prefabName = idParts[^1];
        
        Directory.CreateDirectory(pathFolder);
        
        GameObject go = new GameObject(prefabName);
        CMSEntityPfb component = go.AddComponent<CMSEntityPfb>();
        component.entityState = entity;
        
        string prefabPath = Path.Combine(pathFolder, $"{prefabName}.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        UnityEngine.Object.DestroyImmediate(go);
    }
}

/// <summary>
/// Окно отдельного `Inspector` для выбранного CMS Prefab
/// </summary>
public class CMSEntityInspector : EditorWindow
{
    private GameObject selectedPrefab;
    private GameObject prefabRoot;
    private CMSEntityPfb selectedEntity;
    private SerializedObject serializedEntity;
    private UnityEditor.Editor entityEditor;
    private Vector2 scrollPos;

    public void SetSelectedPrefab(GameObject prefab)
    {
        Cleanup();

        selectedPrefab = prefab;

        string path = AssetDatabase.GetAssetPath(prefab);
        prefabRoot = PrefabUtility.LoadPrefabContents(path);

        if (prefabRoot == null)
        {
            Debug.LogError("Failed to load prefab root.");
            return;
        }

        selectedEntity = prefabRoot.GetComponent<CMSEntityPfb>();
        if (selectedEntity == null)
        {
            Debug.LogWarning("CMSEntityPfb not found on prefab.");
            return;
        }

        serializedEntity = new SerializedObject(selectedEntity);
        entityEditor = UnityEditor.Editor.CreateEditor(selectedEntity);

        Repaint();
    }

    private void OnGUI()
    {
        if (selectedEntity == null || entityEditor == null)
        {
            EditorGUILayout.HelpBox("No prefab selected.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("Selected Prefab:", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CMSEntity", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        var entitySprite = selectedEntity.entityState.GetSprite();
        if (entitySprite != null)
        {
            Rect spriteRect = GUILayoutUtility.GetRect(100, 100, GUILayout.Width(100), GUILayout.Height(100));
            CMSEntityExplorer.GUIDrawSprite(spriteRect, entitySprite);
        }

        serializedEntity.Update();
        entityEditor.OnInspectorGUI();
        serializedEntity.ApplyModifiedProperties();

        EditorGUILayout.Space();

        if (GUILayout.Button("💾 Save Changes"))
        {
            SaveChangesToPrefab();
            CMS.Unload();
            CMS.Init();
        }

        
        
        if (GUILayout.Button("📤 Export to JSON"))
        {
            try
            {
                var original = selectedEntity.entityState;
                var filteredComponents = new List<ICmsComponentDefinition>();

                foreach (var component in original.components)
                {
                    if (component == null) continue;
                    if (component.GetType().GetCustomAttribute<CmsComponentIgnoreAttribute>() != null)
                        continue;

                    filteredComponents.Add(component);
                }

                var exportEntity = new CmsEntity()
                {
                    id = original.id,
                    components = filteredComponents
                };

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };

                string json = JsonConvert.SerializeObject(exportEntity, settings);
                string defaultName = (exportEntity.id ?? "UnnamedEntity").Replace("/", "_") + ".json";

                string path = EditorUtility.SaveFilePanel("Export CMS Entity to JSON", 
                    ProjectPath.MainFolder + "ExportedCmsEntities", defaultName, "json");

                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(path, json);
                    AssetDatabase.Refresh();
                    Debug.Log($"📁 Exported entity to: {path}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Export failed: " + ex.Message);
            }
        }

        
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void SaveChangesToPrefab()
    {
        if (prefabRoot == null || selectedEntity == null) return;

        string path = AssetDatabase.GetAssetPath(selectedPrefab);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log($"Saved prefab to: {path}");

        Cleanup();

        SetSelectedPrefab(selectedPrefab);
    }

    private void Cleanup()
    {
        if (prefabRoot != null)
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            prefabRoot = null;
        }

        selectedEntity = null;
        serializedEntity = null;
        entityEditor = null;
    }

    private void OnDisable()
    {
        Cleanup();
    }
}
