using Hotfix.Framework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EGFramework.Editor.UI
{
    [CustomEditor(typeof(ExtendImage), true), CanEditMultipleObjects]
    public class ExtendImageEditor : ImageEditor
    {
        private SerializedProperty m_Sprite;
        private SerializedProperty m_Type;
        private SerializedProperty m_PreserveAspect;
        private SerializedProperty m_UseSpriteMesh;

        private AnimBool m_ShowImgType;
        private SerializedProperty m_FillMethod;
        private SerializedProperty m_SlicedClipMode;
        private SerializedProperty m_AsyncLoadingSprite;

        private SerializedProperty m_RoundedMode;
        private SerializedProperty m_RoundedTriangleNum ;
        private SerializedProperty m_RoundedRadius;
        private SerializedProperty m_RoundedLeftTop;
        private SerializedProperty m_RoundedRightTop;
        private SerializedProperty m_RoundedLeftBottom;
        private SerializedProperty m_RoundedRightBottom;
        private static bool m_ShowRoundedCorner = false;

        protected override void OnEnable()
        {
            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Type = serializedObject.FindProperty("m_Type");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
            m_UseSpriteMesh = serializedObject.FindProperty("m_UseSpriteMesh");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_SlicedClipMode = serializedObject.FindProperty("m_SlicedClipMode");
            m_AsyncLoadingSprite = serializedObject.FindProperty("m_AsyncLoadingSprite");
            m_ShowImgType = new AnimBool(m_Sprite.objectReferenceValue != null);

            m_RoundedMode = serializedObject.FindProperty("m_RoundedMode");
            m_RoundedTriangleNum = serializedObject.FindProperty("m_RoundedTriangleNum");
            m_RoundedRadius = serializedObject.FindProperty("m_RoundedRadius");
            m_RoundedLeftTop = serializedObject.FindProperty("m_RoundedLeftTop");
            m_RoundedRightTop = serializedObject.FindProperty("m_RoundedRightTop");
            m_RoundedLeftBottom = serializedObject.FindProperty("m_RoundedLeftBottom");
            m_RoundedRightBottom = serializedObject.FindProperty("m_RoundedRightBottom");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SpriteGUI();

            EditorGUILayout.PropertyField(m_AsyncLoadingSprite);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();

            m_ShowImgType.target = m_Sprite.objectReferenceValue != null;
            if (EditorGUILayout.BeginFadeGroup(m_ShowImgType.faded))
                TypeGUI();
            EditorGUILayout.EndFadeGroup();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;

                if ((Image.Type)m_Type.enumValueIndex == Image.Type.Simple)
                {
                    EditorGUILayout.PropertyField(m_UseSpriteMesh);
                    
                }
                if ((Image.Type)m_Type.enumValueIndex == Image.Type.Filled)
                {
                    if ((Image.FillMethod)m_FillMethod.enumValueIndex == Image.FillMethod.Horizontal ||
                        (Image.FillMethod)m_FillMethod.enumValueIndex == Image.FillMethod.Vertical)
                        EditorGUILayout.PropertyField(m_SlicedClipMode);
                }

                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            //m_RoundedMode
            if ((Image.Type)m_Type.enumValueIndex == Image.Type.Simple)
            {
                RoundedGUI();
            }
            

            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void RoundedGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_RoundedMode);
            if (m_RoundedMode.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_RoundedTriangleNum);
                EditorGUILayout.PropertyField(m_RoundedRadius);
                m_ShowRoundedCorner = EditorGUILayout.Foldout(m_ShowRoundedCorner, "Rounded Corner");
                if (m_ShowRoundedCorner)
                {
                    EditorGUI.indentLevel ++;
                    EditorGUILayout.PropertyField(m_RoundedLeftTop);
                    EditorGUILayout.PropertyField(m_RoundedRightTop);
                    EditorGUILayout.PropertyField(m_RoundedLeftBottom);
                    EditorGUILayout.PropertyField(m_RoundedRightBottom);
                    EditorGUI.indentLevel--;
                }
                //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //EditorGUILayout.LabelField("Rounded Corner", GUI.skin.FindStyle("IN BigTitle Post"));
                //EditorGUILayout.PropertyField(m_RoundedLeftTop);
                //EditorGUILayout.PropertyField(m_RoundedRightTop);
                //EditorGUILayout.PropertyField(m_RoundedLeftBottom);
                //EditorGUILayout.PropertyField(m_RoundedRightBottom);
                //EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
        }

        private void SetShowNativeSize(bool instant)
        {
            var type = (Image.Type)m_Type.enumValueIndex;
            var showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled) && m_Sprite.objectReferenceValue != null;
            base.SetShowNativeSize(showNativeSize, instant);
        }
    }
}