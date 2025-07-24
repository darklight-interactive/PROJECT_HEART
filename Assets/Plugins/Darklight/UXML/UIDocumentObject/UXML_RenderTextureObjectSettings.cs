using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{
    [CreateAssetMenu(menuName = "Darklight/UXML/RenderTexturePreset")]
    public class UXML_RenderTextureObjectSettings : ScriptableObject
    {
        public RenderTexture renderTextureAsset;

        [Header("Panel Configuration")]
        [Tooltip("Width of the panel in pixels.")]
        [Range(1, 4000)]
        public int panelWidth = 1280;

        [Tooltip("Height of the panel in pixels.")]
        [Range(1, 4000)]
        public int panelHeight = 720;

        [Tooltip("Scale of the panel (like zoom in a browser).")]
        [Range(0.01f, 10.0f)]
        public float panelScale = 1.0f;

        [Tooltip("Pixels per world unit. Determines the real-world size of the panel.")]
        [Range(1, 1000)]
        public float pixelsPerUnit = 500.0f;

        public void Reset()
        {
            panelWidth = 1280;
            panelHeight = 720;
            panelScale = 1.0f;
            pixelsPerUnit = 500.0f;
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(UXML_RenderTextureObjectSettings))]
        public class UXML_RenderTextureObjectSettingsCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            UXML_RenderTextureObjectSettings _script;

            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (UXML_RenderTextureObjectSettings)target;
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Reset"))
                {
                    _script.Reset();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }
}
