using UnityEngine;
using UnityEditor;

namespace UAP
{
    [CustomEditor(typeof(Google_TTS))]
    public class Google_TTS_Inspector : Accessibility_InspectorShared
    {

        private SerializedProperty m_GoogleTTSAPIKey;

        static bool showConfigs = false;

        void OnEnable()
        {
            m_GoogleTTSAPIKey = serializedObject.FindProperty("m_GoogleTTSAPIKey");
        }

        public override void OnInspectorGUI()
        {
            SetupGUIStyles();
            serializedObject.Update();

            EditorGUILayout.Separator();

            showConfigs = DrawSectionHeader("Keys and Configurations", showConfigs);

            if (showConfigs)
            {
                EditorGUILayout.PropertyField(m_GoogleTTSAPIKey, new GUIContent("Google TTS API key", "You can provide your Google Cloud API key to activate " +
                    "Google TTS for WebGL. The UAP documentation contains step-by-step instructions on how to get an API key.\nThis is optional - if no key is provided, " +
                    "UAP will use the browser Web Speech API to generate speech (most browsers support this)"));

                // If there is nothing, or only a very short key in the text field (invalid) show some help on how to get the key
                // The key should be 39 characters long - but it's not a hard requirement
                if (m_GoogleTTSAPIKey.stringValue == null || m_GoogleTTSAPIKey.stringValue.Length < 39)
                {
                    if (GUILayout.Button("Get API Key", GUILayout.MaxWidth(120)))
                    {
                        Application.OpenURL("http://www.metalpopgames.com/assetstore/accessibility/doc/WebGL.html");
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspectorSection();
        }
    }
}
