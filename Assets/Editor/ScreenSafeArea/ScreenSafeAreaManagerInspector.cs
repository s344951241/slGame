using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(ScreenSafeAreaManager))]
    internal class ScreenSafeAreaManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty m_SimulateConfigNameProp = null;
        private SerializedProperty m_DefaultConfigAssetProp = null;
        private SerializedProperty m_PersistentConfigRelativePathProp = null;
        private SerializedProperty m_SimulateOrientationProp = null;
        private string[] m_ConfigNames = null;

        private void OnEnable()
        {
            m_SimulateConfigNameProp = serializedObject.FindProperty("m_SimulateConfigName");
            m_DefaultConfigAssetProp = serializedObject.FindProperty("m_DefaultConfigAsset");
            m_PersistentConfigRelativePathProp = serializedObject.FindProperty("m_PersistentConfigRelativePath");
            m_SimulateOrientationProp = serializedObject.FindProperty("m_SimulateOrientation");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                DrawDefaultConfigAssetProp();
                DrawPersistentConfigRelativePathProp();
                DrawSimulateConfigNameProp();
                DrawSimulateOrientationProp();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawSimulateOrientationProp()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_SimulateOrientationProp);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EnsureAndSetGameViewAspectRatio(m_SimulateConfigNameProp.stringValue, (ScreenOrientation)m_SimulateOrientationProp.intValue);
            }
        }

        private void DrawDefaultConfigAssetProp()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_DefaultConfigAssetProp);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                m_ConfigNames = null;
            }
        }

        private void DrawPersistentConfigRelativePathProp()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_PersistentConfigRelativePathProp);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void EnsureConfigNames()
        {
            if (m_ConfigNames != null)
            {
                return;
            }

            (target as ScreenSafeAreaManager).ReloadDefaultConfigs();
            var configNames = new List<string>();
            configNames.Add("None");
            configNames.AddRange((target as ScreenSafeAreaManager).GetConfigNames());
            m_ConfigNames = configNames.ToArray();
        }

        private void DrawSimulateConfigNameProp()
        {
            EnsureConfigNames();

            var oldValue = m_SimulateConfigNameProp.stringValue;
            int oldIndex = Array.IndexOf(m_ConfigNames, oldValue);

            var newIndex = EditorGUILayout.Popup("Simulation Config Name", oldIndex < 0 ? 0 : oldIndex, m_ConfigNames);
            var newValue = oldValue;
            if (newIndex != oldIndex)
            {
                newValue = newIndex == 0 ? string.Empty : m_ConfigNames[newIndex];
                m_SimulateConfigNameProp.stringValue = newValue;
                serializedObject.ApplyModifiedProperties();
                EnsureAndSetGameViewAspectRatio(newValue, (ScreenOrientation)m_SimulateOrientationProp.intValue);
            }
        }

        private void EnsureAndSetGameViewAspectRatio(string configName, ScreenOrientation screenOrientation)
        {
            var config = (target as ScreenSafeAreaManager).GetConfig(configName);
            if (config == null)
            {
                return;
            }

            if (screenOrientation == ScreenOrientation.AutoRotation || screenOrientation == ScreenOrientation.Unknown)
            {
                return;
            }

            var width = config.AspectRatio.Width;
            var height = config.AspectRatio.Height;
            bool isPortrait = false;
            if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
            {
                width = config.AspectRatio.Height;
                height = config.AspectRatio.Width;
                isPortrait = true;
            }

            var groupType = GameViewSizeUtils.GetCurrentGroupType();
            var index = GameViewSizeUtils.FindSize(groupType, GameViewSizeType.AspectRatio, width, height);
            if (index < 0)
            {
                GameViewSizeUtils.AddCustomSize(GameViewSizeType.AspectRatio, groupType, width, height, configName + " " + (isPortrait ? "Portrait" : "Landscape"));
                index = GameViewSizeUtils.FindSize(groupType, GameViewSizeType.AspectRatio, width, height);
            }

            GameViewSizeUtils.SetSize(index);
        }
    }
