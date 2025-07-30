using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ResourcePathGenerator
{
    private const string ResourcesFolder = ProjectPath.MainFolder + "Resources/CMS";
    private const string OutputPath = ProjectPath.MainFolder + "Code/Infrastructure/CMS/Generate/CmsPath.cs";

    public static void PathGenerate()
    {
        if (!Directory.Exists(ResourcesFolder))
        {
            Debug.LogError("Resources folder not found! Make sure 'Assets/Resources/CMS' exists.");
            return;
        }

        var resourceFiles = Directory.GetFiles(ResourcesFolder, "*.*", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith(".meta"))
            .Select(file => file.Replace("\\", "/"))
            .Select(file => file.Substring(ResourcesFolder.Length + 1)) // Remove "Assets/Resources/CMS/"
            .Select(file => Path.ChangeExtension(file, null)) // Remove extension
            .ToList();

        Dictionary<string, Dictionary<string, List<string>>> categorizedFiles = new();
        List<string> rootFiles = new();

        foreach (var file in resourceFiles)
        {
            string[] parts = file.Split('/');
            if (parts.Length == 1)
            {
                rootFiles.Add(parts[0]);
                continue;
            }
            
            string mainCategory = parts[0];
            string subCategory = parts.Length > 2 ? parts[1] : "";
            string fileName = parts.Last();

            if (!categorizedFiles.ContainsKey(mainCategory))
                categorizedFiles[mainCategory] = new Dictionary<string, List<string>>();
            
            if (!categorizedFiles[mainCategory].ContainsKey(subCategory))
                categorizedFiles[mainCategory][subCategory] = new List<string>();
            
            categorizedFiles[mainCategory][subCategory].Add(fileName);
        }

        string classContent = "// Auto-generated file\n";
        classContent += "public static class CMSPath\n";
        classContent += "{\n";

        foreach (var file in rootFiles)
            classContent += $"    public const string {file} = \"{file}\";\n";

        foreach (var mainCategory in categorizedFiles.Keys.OrderBy(k => k))
        {
            classContent += $"    public static class {mainCategory}\n    {{\n";
            List<string> mainCategoryAll = new List<string>();
            
            foreach (var subCategory in categorizedFiles[mainCategory].Keys.OrderBy(k => k))
            {
                var fileList = categorizedFiles[mainCategory][subCategory];
                if (!string.IsNullOrEmpty(subCategory))
                {
                    classContent += $"        public static class {subCategory}\n        {{\n";
                    List<string> subCategoryAll = new List<string>();
                    foreach (var file in fileList)
                    {
                        classContent += $"            public const string {file} = \"{mainCategory}/{subCategory}/{file}\";\n";
                        subCategoryAll.Add(file);
                        mainCategoryAll.Add($"{subCategory}.{file}");
                    }
                    classContent += $"            public static string[] all = {{ {string.Join(", ", subCategoryAll)} }};\n";
                    classContent += "        }\n";
                }
                else
                {
                    foreach (var file in fileList)
                    {
                        classContent += $"        public const string {file} = \"{mainCategory}/{file}\";\n";
                        mainCategoryAll.Add(file);
                    }
                }
            }
            classContent += $"        public static string[] all = {{ {string.Join(", ", mainCategoryAll)} }};\n";
            classContent += "    }\n";
        }

        classContent += "}\n\n";
        classContent += "public static class CMSEntityes\n";
        classContent += "{\n";

        foreach (var file in rootFiles)
            classContent += $"    public static CmsEntity {file} => CMSPath.{file}.GetCmsEntity();\n";

        foreach (var mainCategory in categorizedFiles.Keys.OrderBy(k => k))
        {
            classContent += $"    public static class {mainCategory}\n    {{\n";
            foreach (var subCategory in categorizedFiles[mainCategory].Keys.OrderBy(k => k))
            {
                var fileList = categorizedFiles[mainCategory][subCategory];
                if (!string.IsNullOrEmpty(subCategory))
                {
                    classContent += $"        public static class {subCategory}\n        {{\n";
                    foreach (var file in fileList)
                        classContent += $"            public static CmsEntity {file} => CMSPath.{mainCategory}.{subCategory}.{file}.GetCmsEntity();\n";
                    classContent += "        }\n";
                }
                else
                {
                    foreach (var file in fileList)
                        classContent += $"        public static CmsEntity {file} => CMSPath.{mainCategory}.{file}.GetCmsEntity();\n";
                }
            }
            classContent += "    }\n";
        }

        classContent += "}\n";

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        File.WriteAllText(OutputPath, classContent);

        AssetDatabase.Refresh();
        Debug.Log("Resource paths generated successfully!");
    }
}