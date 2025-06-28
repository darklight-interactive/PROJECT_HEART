//using Darklight.UnityExt.Input.Editor;
using Darklight.UnityExt.Utility;
using UnityEditor;
using UnityEngine;

namespace RemixSurvivors.Survivor.Editor
{
    /*
    [CustomEditor(typeof(SurvivorMovementController))]
    public class SurvivorMovementControllerCustomEditor : UniversalInputControllerCustomEditor
    {
        const string ASSET_PATH = "Assets/Resources/RemixSurvivors/Movement/";
        const string DEFAULT_SETTINGS_NAME = "DefaultSurvivorMovementSettings";

        public SurvivorMovementController survivorController;
        public SurvivorMovementSettings survivorMovementSettings;

        public void OnEnable()
        {
            survivorController = (SurvivorMovementController)target;
            survivorController.Initialize();
        }

        public override void OnInspectorGUI()
        {
            survivorController = (SurvivorMovementController)target;
            
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            SerializedProperty settingsProperty = serializedObject.FindProperty("_settings");
            survivorMovementSettings =
                settingsProperty.objectReferenceValue as SurvivorMovementSettings;
            if (survivorMovementSettings == null)
            {
                if (GUILayout.Button("Create Default Settings"))
                {
                    SurvivorMovementSettings settings =
                        ScriptableObjectUtility.CreateOrLoadScriptableObject<SurvivorMovementSettings>(
                            ASSET_PATH,
                            DEFAULT_SETTINGS_NAME
                        );
                    settingsProperty.objectReferenceValue = settings;
                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                survivorController.Initialize();
            }
        }

        protected override void DrawHeaderButtons()
        {
            base.DrawHeaderButtons();
            if (GUILayout.Button("Open Survivor Controller Debug Window"))
            {
                DebugWindow.ShowWindow(survivorController);
            }
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
                return;

            if (survivorController == null)
                return;

            float rayLength = 2f;
            Vector3 origin = survivorController.transform.position + Vector3.up * 0.1f;

            // Current Velocity (Blue)
            Handles.color = Color.blue;
            Handles.DrawLine(
                origin,
                origin + survivorController.CurrentVelocity.Combined.normalized * rayLength
            );
            Handles.Label(
                origin + survivorController.CurrentVelocity.Combined.normalized * rayLength,
                "Velocity"
            );

            // Horizontal Velocity (Cyan)
            Handles.color = Color.cyan;
            Vector3 horzVel = new Vector3(
                survivorController.CurrentVelocity.Horizontal.x,
                0,
                survivorController.CurrentVelocity.Horizontal.y
            );
            Handles.DrawLine(origin, origin + horzVel.normalized * rayLength);
            Handles.Label(origin + horzVel.normalized * rayLength, "HorzVel");

            // Current Rotation (Green)
            Handles.color = Color.green;
            Vector3 currentForward = survivorController.CurrentRotation * Vector3.forward;
            Handles.DrawLine(origin, origin + currentForward * rayLength);
            Handles.Label(origin + currentForward * rayLength, "Current");

            // Target Rotation (Red)
            Handles.color = Color.red;
            Vector3 targetForward = survivorController.TargetRotation * Vector3.forward;
            Handles.DrawLine(origin, origin + targetForward * rayLength);
            Handles.Label(origin + targetForward * rayLength, "Target");
        }
*/

    #region < PRIVATE_NESTED_CLASS > [[ Debug Window ]] ================================================================
    public class DebugWindow : EditorWindow
    {
        const string WINDOW_TITLE = "Survivor Movement Debug";
        private SurvivorMovementController _target;
        private Vector2 _scrollPosition;
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _activeStyle;
        private GUIStyle _inactiveStyle;

        public static void ShowWindow(SurvivorMovementController target)
        {
            DebugWindow window = GetWindow<DebugWindow>(WINDOW_TITLE);
            window._target = target;
            window.minSize = new Vector2(300, 200);
        }

        private void InitStyles()
        {
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = MakeTexture(new Color(0f, 0f, 0f, 0.7f)) },
                    padding = new RectOffset(10, 10, 10, 10)
                };
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = Color.white },
                    fontSize = 12
                };
            }

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(_labelStyle)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold
                };
            }

            if (_activeStyle == null)
            {
                _activeStyle = new GUIStyle(_labelStyle) { normal = { textColor = Color.green } };
            }

            if (_inactiveStyle == null)
            {
                _inactiveStyle = new GUIStyle(_labelStyle) { normal = { textColor = Color.red } };
            }
        }

        private Texture2D MakeTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private void OnGUI()
        {
            // Try to find the controller if reference is lost
            if (_target == null)
            {
                _target = FindAnyObjectByType<SurvivorMovementController>();
                if (_target == null)
                {
                    EditorGUILayout.HelpBox(
                        "No SurvivorMovementController found in scene.",
                        MessageType.Warning
                    );
                    return;
                }
            }
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot debug in edit mode.", MessageType.Error);
                return;
            }

            InitStyles();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.BeginVertical(_boxStyle);

            // Header
            EditorGUILayout.LabelField(WINDOW_TITLE, _headerStyle);
            EditorGUILayout.Space(5);

            // Current State
            EditorGUILayout.LabelField("Current State", _headerStyle);
            if (_target.StateMachine != null && _target.StateMachine.CurrentFiniteState != null)
            {
                EditorGUILayout.LabelField(
                    $"State: {_target.StateMachine.CurrentFiniteState.StateType}",
                    _labelStyle
                );
            }
            else
            {
                EditorGUILayout.LabelField("No state machine found.", _labelStyle);
            }

            EditorGUILayout.Space(10);

            // Ground State
            EditorGUILayout.LabelField("Ground State", _headerStyle);
            EditorGUILayout.LabelField(
                "On Ground Sensor",
                _target.GroundSensor.IsColliding ? _activeStyle : _inactiveStyle
            );

            EditorGUILayout.LabelField(
                "On Grindable Sensor",
                _target.GrindableSensor.IsColliding ? _activeStyle : _inactiveStyle
            );

            EditorGUILayout.Space(10);

            // Combined Values
            EditorGUILayout.LabelField("Combined Values", _headerStyle);
            EditorGUILayout.LabelField(
                $"Target Direction: {_target.TargetDirection.Combined:F2}",
                _labelStyle
            );
            EditorGUILayout.LabelField(
                $"Current Direction: {_target.CurrentDirection.Combined:F2}",
                _labelStyle
            );
            EditorGUILayout.LabelField(
                $"Target Velocity: {_target.TargetVelocity.Combined:F2}",
                _labelStyle
            );
            EditorGUILayout.LabelField(
                $"Current Velocity: {_target.CurrentVelocity.Combined:F2}",
                _labelStyle
            );

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            // Repaint every frame while in play mode
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
    #endregion
}
//}
