using UnityEditor;
using UnityEngine;

namespace Darklight.Inky.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(InkyStoryManager), true)]
    public class InkyStoryManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        InkyStoryManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (InkyStoryManager)target;
            //_script.Initialize();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                //_script.Initialize();
            }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
