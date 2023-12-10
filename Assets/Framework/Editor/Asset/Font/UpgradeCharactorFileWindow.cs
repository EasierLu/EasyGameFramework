using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CharacterSetCreator : EditorWindow
{
    [MenuItem("Window/CharacterSetCreator")]
    private static void Init()
    {
        CharacterSetCreator ucfw = GetWindow<CharacterSetCreator>(false, "CharacterSetCreator", true);
        ucfw.minSize = new Vector2(1000, 600);
    }

    private string m_TargetFilePath;
    private List<string> m_CharacterSetFiles;

    private void OnGUI()
    {
        if (m_CharacterSetFiles == null)
        {
            m_CharacterSetFiles = new List<string>();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target File:", GUILayout.Width(80));
        GUILayout.TextField(m_TargetFilePath, GUILayout.Width(800));
        if (GUILayout.Button("Select", GUILayout.Width(100)))
        {
            m_TargetFilePath = EditorUtility.OpenFilePanel("Character File", m_TargetFilePath, "txt");
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        for (int i = 0; i < m_CharacterSetFiles.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{i + 1}:{m_CharacterSetFiles[i]}", GUILayout.Width(900));
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                m_CharacterSetFiles.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add character set", GUILayout.Width(200)))
        {
            string path = EditorUtility.OpenFilePanel("Character File", "", "txt");
            if(!string.IsNullOrEmpty(path) && !m_CharacterSetFiles.Contains(path)) 
            {
                m_CharacterSetFiles.Add(path);
            }
        }
        GUILayout.EndHorizontal();
        bool canGen = m_CharacterSetFiles != null && m_CharacterSetFiles.Count > 0 && !string.IsNullOrEmpty(m_TargetFilePath);
        EditorGUI.BeginDisabledGroup(!canGen);
        if (GUILayout.Button("Create or save"))
        {
            string combinedChar = CombineCharacter(m_TargetFilePath, m_CharacterSetFiles);

            SaveCharacters(m_TargetFilePath, combinedChar);
        }
        EditorGUI.EndDisabledGroup();
    }

    private string CombineCharacter(string targetFile, List<string> characterSetFiles)
    {
        HashSet<char> chars = new HashSet<char>();
        chars.Add('¡õ');
        string content = File.ReadAllText(targetFile);
        AddCharToHashSet(content, chars);
        foreach (var characterSet in characterSetFiles) 
        {
            content = File.ReadAllText(characterSet);
            AddCharToHashSet(content, chars);
        }
        List<char> charList = new List<char>(chars);
        charList.Sort();
        return new string(charList.ToArray());
    }

    private void AddCharToHashSet(string content, HashSet<char> chars)
    {
        int length = content.Length;
        for (int i = 0; i < length; i++)
        {
            var c = content[i];
            if (!chars.Contains(c) && c != '\n' && c != '\r' && c != '\t')
            { 
                chars.Add(c);
            }
        }
    }

    private void SaveCharacters(string savePath, string content)
    {
        File.WriteAllText(savePath, content);
        AssetDatabase.Refresh();
    }
}