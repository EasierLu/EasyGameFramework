using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class CustomFileEditor
{

    [OnOpenAsset(0)]
    public static bool OnDoubleClick(int instanceID, int line, int column)
    {
        string fullPath = InstanceIDToFullPath(instanceID);
        string fileExtension = Path.GetExtension(fullPath);

        switch (fileExtension)
        {
            case ".shader":
            case ".hlsl":
            case ".cg":
            case ".json":
                return OpenWithVSCode(fullPath, line, column);
            default:
                //UnityEngine.Debug.Log(fileExtension);
                return false;
        }
    }

    #region VS Code
    private static bool m_IsInstalledVSCode = false;
    private const string kOpenWithVSCode = "Assets/VS Code/Open";
    private const string kCompareWithVSCode = "Assets/VS Code/Compare";

    [MenuItem(kOpenWithVSCode, priority = 10000)]
    public static void OpenSelectWithVSCode()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            return;
        }
        List<string> folders = new List<string>();
        List<string> files = new List<string>();
        foreach (var item in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(item);
            string fullPath = Path.GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                folders.Add(fullPath);
            }
            else
            {
                files.Add(fullPath);
            }
        }
        if (folders.Count > 0)
        {
            OpenWithVSCode(folders.ToArray(), new string[] { "-a" });
        }
        else
        {
            OpenWithVSCode(files.ToArray());
        }
    }

    [MenuItem(kOpenWithVSCode, true, priority = 10000)]
    public static bool OpenSelectWithVSCodeValidate()
    {
        return Selection.assetGUIDs.Length > 0;
    }

    [MenuItem(kCompareWithVSCode, priority = 10001)]
    public static void CompareWithVSCode()
    {
        if (Selection.instanceIDs.Length == 2)
        {
            string[] fullPaths = new string[Selection.instanceIDs.Length];
            for (int i = 0; i < Selection.instanceIDs.Length; i++)
            {
                fullPaths[i] = InstanceIDToFullPath(Selection.instanceIDs[i]);
            }
            OpenWithVSCode(fullPaths, new string[] { "-d" });
        }
        else
        {
            UnityEngine.Debug.Log("Select 2 files to compare");
        }
    }

    [MenuItem(kCompareWithVSCode, true, priority = 10001)]
    public static bool CompareWithVSCodeValidate()
    {
        return Selection.instanceIDs.Length == 2;
    }

    public static bool OpenWithVSCode(string fullPath, int line, int column)
    {
        return OpenWithVSCode(new string[] { $"{fullPath}:{Mathf.Max(0, line)}:{Mathf.Max(0, column)}" }, new string[] { "-g" });
    }

    public static bool OpenWithVSCode(string fullPath)
    {
        return OpenWithVSCode(new string[] { fullPath });
    }

    public static bool OpenWithVSCode(string[] paths, string[] args = null)
    {
        if (!IsInstalledVSCode())
        {
            return false;
        }

        Process process = new Process();
        process.StartInfo.FileName = "code";
        string arguments = string.Join(" ", paths.Select(path => $"\"{path}\"").ToArray());
        if(args != null)
        {
            arguments += " " + string.Join(" ", args);
        };

        process.StartInfo.Arguments = arguments;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //UnityEngine.Debug.Log(process.StartInfo.FileName + " " + process.StartInfo.Arguments);
        process.Start();

        return true;
    }
    
    private static bool IsInstalledVSCode()
    {
        if (!m_IsInstalledVSCode)
        {
            string environment = Environment.GetEnvironmentVariable("PATH");
            m_IsInstalledVSCode = environment.Contains("VS Code");

            if (!m_IsInstalledVSCode)
            {
                UnityEngine.Debug.LogError("Please install Visual Studio Code. If already installed, please restart Unity Editor and Unity Hub.");
                return false;
            }
        }

        return true;
    }
    #endregion

    private static string InstanceIDToFullPath(int instanceID)
    {
        string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        return Path.GetFullPath(path);
    }
}