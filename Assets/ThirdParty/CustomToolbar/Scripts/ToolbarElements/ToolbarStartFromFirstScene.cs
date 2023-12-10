using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityToolbarExtender;

[Serializable]
internal class ToolbarStartFromFirstScene : BaseToolbarElement {
	private static GUIContent startFromFirstSceneBtn;

	public override string NameInList => "[Button] Start from first scene";

	public override void Init() {
		EditorApplication.playModeStateChanged += LogPlayModeState;
        string iconPath = AssetDatabase.GUIDToAssetPath("c375918ef372b6b44888914ffa2b20c1");
        startFromFirstSceneBtn = new GUIContent((Texture2D)AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D)), "Start from 1 scene");
	}

	protected override void OnDrawInList(Rect position) {

	}

	protected override void OnDrawInToolbar() {
		if (GUILayout.Button(startFromFirstSceneBtn, ToolbarStyles.commandButtonStyle)) {
			if (!EditorApplication.isPlaying) {
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorPrefs.SetInt("LastActiveSceneToolbar", EditorSceneManager.GetActiveScene().buildIndex);
				EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0));
			}

			EditorApplication.isPlaying = !EditorApplication.isPlaying;
		}
	}

	private static void LogPlayModeState(PlayModeStateChange state) {
		if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.HasKey("LastActiveSceneToolbar")) {
			EditorSceneManager.OpenScene(
				SceneUtility.GetScenePathByBuildIndex(EditorPrefs.GetInt("LastActiveSceneToolbar")));
			EditorPrefs.DeleteKey("LastActiveSceneToolbar");
		}
	}
}
