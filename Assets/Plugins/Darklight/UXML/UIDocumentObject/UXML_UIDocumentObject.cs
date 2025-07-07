using System.Collections.Generic;
using Darklight.Editor;
using Darklight.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes.Editor;
#endif

namespace Darklight.UXML
{
    /// <summary>
    /// A MonoBehaviour that handles the initialization of a UIDocument and manages its elements.
    /// It is suggested that you create a new ScriptableObject that inherits from UXML_UIDocumentPreset
    /// and assign it to the UIDocumentObject in the inspector.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(UIDocument))]
    public class UXML_UIDocumentObject : MonoBehaviour, IUnityEditorListener
    {
        // << PUBLIC ACCESSORS >> //
        [SerializeField, Expandable]
        [CreateAsset("NewUIDocumentPreset", "Assets/Resources/Darklight/UXML/")]
        private UXML_UIDocumentPreset _uiDocumentPreset;

        public UIDocument Document => GetComponent<UIDocument>();
        public UXML_UIDocumentPreset Preset => _uiDocumentPreset;
        public VisualElement Root => Document.rootVisualElement;
        public bool IsVisible { get; protected set; }

        protected virtual void OnEditorReloaded()
        {
            Initialize();
        }

        protected virtual void OnInitialized() { }

        public virtual void Initialize(
            UXML_UIDocumentPreset preset,
            bool clonePanelSettings = false
        )
        {
            this._uiDocumentPreset = preset;
            if (preset == null)
            {
                Debug.LogError("No preset assigned to UIDocumentObject", this.gameObject);
                return;
            }

            Document.visualTreeAsset = preset.visualTreeAsset;

            if (clonePanelSettings)
            {
                // Create a new PanelSettings instance
                PanelSettings clonedPanelSettings =
                    ScriptableObject.CreateInstance<PanelSettings>();

                // Copy properties from the original PanelSettings to the new one
                CopyPanelSettings(preset.panelSettings, clonedPanelSettings);
                Document.panelSettings = clonedPanelSettings;
            }
            else
            {
                Document.panelSettings = preset.panelSettings;
            }

            // Assign the layer
            gameObject.layer = LayerMask.NameToLayer("UI");

            //Debug.Log($"Initialized UIDocumentObject with preset {preset.name}");
        }

        public virtual void Initialize()
        {
            Initialize(_uiDocumentPreset);
        }

        /// <summary>
        /// Query the root element for a VisualElement of the given type with an optional tag or class.
        /// </summary>
        /// <typeparam name="T">The type of VisualElement to query for.</typeparam>
        /// <param name="tagOrClass">Optional tag or class name to further refine the query.</param>
        /// <returns>The first matching element, or null if no match is found.</returns>
        public T ElementQuery<T>(string tagOrClass = null)
            where T : VisualElement
        {
            UQueryBuilder<T> query = Root.Query<T>(tagOrClass);
            return query.First();
        }

        /// <summary>
        /// Query the root element for all VisualElements of the given type with an optional tag or class.
        /// </summary>
        /// <typeparam name="T">The type of VisualElement to query for.</typeparam>
        /// <param name="tagOrClass">Optional tag or class name to further refine the query.</param>
        /// <returns>An enumerable of all matching elements.</returns>
        public IEnumerable<T> ElementQueryAll<T>(string tagOrClass = null)
            where T : VisualElement
        {
            HashSet<T> elements = new HashSet<T>();
            Root.Query<T>(tagOrClass).ForEach(element => elements.Add(element));
            return elements;
        }

        /// <summary>
        /// Toggle the visibility of the root element.
        /// </summary>
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
            Root.visible = IsVisible;
        }

        /// <summary>
        /// Set the visibility of the root element directly.
        /// </summary>
        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
            Root.visible = IsVisible;
        }

        private void CopyPanelSettings(PanelSettings source, PanelSettings destination)
        {
            // Scale and Resolution Settings
            destination.scaleMode = source.scaleMode;
            destination.referenceResolution = source.referenceResolution;
            destination.screenMatchMode = source.screenMatchMode;
            destination.match = source.match;

            // Display and Sorting Settings
            destination.sortingOrder = source.sortingOrder;
            destination.targetDisplay = source.targetDisplay;
            destination.sortingOrder = source.sortingOrder;

            // Render Target and Clear Settings
            destination.targetTexture = source.targetTexture;
            destination.clearColor = source.clearColor;
            destination.clearDepthStencil = source.clearDepthStencil;
            destination.targetDisplay = source.targetDisplay;

            // Scale Factor Settings
            destination.referenceDpi = source.referenceDpi;
            destination.fallbackDpi = source.fallbackDpi;

            // DPI Scale Settings
            destination.referenceSpritePixelsPerUnit = source.referenceSpritePixelsPerUnit;
            destination.referenceDpi = source.referenceDpi;
            destination.referenceResolution = source.referenceResolution;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UXML_UIDocumentObject), true)]
    public class UXML_UIDocumentObjectCustomEditor : NaughtyInspector
    {
        SerializedObject _serializedObject;
        UXML_UIDocumentObject _script;

        protected override void OnEnable()
        {
            base.OnEnable();
            _serializedObject = new SerializedObject(target);
            _script = (UXML_UIDocumentObject)target;
            _script.Initialize();
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Initialize"))
                    _script.Initialize();

                if (GUILayout.Button("Toggle Visibility"))
                    _script.ToggleVisibility();
            }

            base.OnInspectorGUI();
        }
    }
#endif
}
