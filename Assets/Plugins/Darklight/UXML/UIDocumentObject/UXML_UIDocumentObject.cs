using System;
using System.Collections.Generic;
using Darklight.Editor;
using Darklight.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;
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
    [RequireComponent(typeof(UIDocument))]
    public class UXML_UIDocumentObject : MonoBehaviour, IUnityEditorListener
    {
        UIDocument _uiDocument;
        PanelSettings _panelSettings;

        [SerializeField, Expandable, Required]
        [CreateAsset("NewUIDocumentPreset", "Assets/Resources/Darklight/UXML/")]
        private UXML_UIDocumentPreset _uiDocumentPreset;

        // << PUBLIC ACCESSORS >> //
        public UIDocument UIDocument
        {
            get => _uiDocument;
            protected set => _uiDocument = value;
        }
        public PanelSettings PanelSettings
        {
            get => _panelSettings;
            protected set
            {
                _panelSettings = value;
                _uiDocument.panelSettings = _panelSettings;
            }
        }
        public VisualElement Root => _uiDocument.rootVisualElement;
        public bool IsVisible { get; protected set; }

        public Action<UXML_UIDocumentObject> OnInitialized;

        protected virtual void OnEditorReloaded()
        {
            Initialize();
        }

        public virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the UIDocumentObject with a visual tree asset and panel settings.
        /// </summary>
        /// <param name="visualTreeAsset">The visual tree asset to use for the UIDocument.</param>
        /// <param name="panelSettings">The panel settings to use for the UIDocument.</param>
        public void Initialize(VisualTreeAsset visualTreeAsset, PanelSettings panelSettings)
        {
            // Get or add the UIDocument component & assign the visual tree asset
            _uiDocument = gameObject.GetOrAdd<UIDocument>();
            _uiDocument.visualTreeAsset = visualTreeAsset;

            // Create a new PanelSettings instance
            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();

            // Copy properties from the original PanelSettings to the new one
            CopyPanelSettings(panelSettings, _panelSettings);
            _uiDocument.panelSettings = _panelSettings;

            // Set the layer to UI
            gameObject.layer = LayerMask.NameToLayer("UI");

            // Invoke the OnInitialized event
            OnInitialized?.Invoke(this);
        }

        /// <summary>
        /// Initialize the UIDocumentObject with a preset.
        /// </summary>
        /// <param name="preset">The preset to use for the UIDocument.</param>
        public void Initialize(UXML_UIDocumentPreset preset)
        {
            this._uiDocumentPreset = preset;
            if (preset == null)
            {
                Debug.LogError("No preset assigned to UIDocumentObject", this.gameObject);
                return;
            }

            Initialize(preset.visualTreeAsset, preset.panelSettings);
        }

        /// <summary>
        /// Initialize the UIDocumentObject with the current scriptable object preset.
        /// </summary>
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
