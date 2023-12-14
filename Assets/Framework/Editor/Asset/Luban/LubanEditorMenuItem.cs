using Luban.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorMenuItem
{
    public const string CONFIG_PATH = "Assets/Framework/Editor/Asset/Luban/";

    [MenuItem("Tools/Config/Export Develop", false, 10)]
    public static void ExportConfig_Develop()
    {
        LubanExportConfig config = AssetDatabase.LoadAssetAtPath<LubanExportConfig>(CONFIG_PATH + "ExportConfig.asset");
        config.RunCommand();
    }

    //[MenuItem("Tools/Config/Export Release", false, 11)]
    //public static void ExportConfig_Release()
    //{
    //    LubanExportConfig config = AssetDatabase.LoadAssetAtPath<LubanExportConfig>(CONFIG_PATH + "ExportReleaseConfig.asset");
    //    config.RunCommand();
    //}
}
