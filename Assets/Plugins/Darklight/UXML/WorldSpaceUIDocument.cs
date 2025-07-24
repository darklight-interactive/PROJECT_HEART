using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityUtils;

namespace Darklight.UXML
{
    public class WorldSpaceUIDocument : MonoBehaviour
    {
        #region Fields

        const string k_transparentShader = "Unlit/Transparent";
        const string k_textureShader = "Unlit/Texture";
        const string k_mainTex = "_MainTex";
        static readonly int MainTex = Shader.PropertyToID(k_mainTex);

        MeshRenderer _meshRenderer;
        UIDocument _uiDocument;
        PanelSettings _panelSettings;
        RenderTexture _renderTexture;
        Material _material;

        [Header("Panel Configuration")]
        [Tooltip("Width of the panel in pixels.")]
        [SerializeField]
        int _panelWidth = 1280;

        [Tooltip("Height of the panel in pixels.")]
        [SerializeField]
        int _panelHeight = 720;

        [Tooltip("Scale of the panel (like zoom in a browser).")]
        [SerializeField]
        float _panelScale = 1.0f;

        [Tooltip("Pixels per world unit. Determines the real-world size of the panel.")]
        [SerializeField]
        float _pixelsPerUnit = 500.0f;

        [Header("Dependencies")]
        [Tooltip("Visual tree asset for this panel.")]
        [SerializeField]
        VisualTreeAsset _visualTreeAsset;

        [Tooltip("PanelSettings prefab instance.")]
        [SerializeField]
        PanelSettings _panelSettingsAsset;

        [Tooltip("RenderTexture prefab instance.")]
        [SerializeField]
        RenderTexture _renderTextureAsset;

        #endregion

        void Awake()
        {
            InitializeComponents();
            BuildPanel();
        }

        public void SetLabelText(string label, string text)
        {
            if (_uiDocument.rootVisualElement == null)
            {
                _uiDocument.visualTreeAsset = _visualTreeAsset;
            }

            // Consider caching the label element for better performance
            _uiDocument.rootVisualElement.Q<Label>(label).text = text;
        }

        void InitializeComponents()
        {
            InitializeMeshRenderer();

            // Optionally add a box collider to the object
            // BoxCollider boxCollider = gameObject.GetOrAdd<BoxCollider>();
            // boxCollider.size = new Vector3(1, 1, 0);

            MeshFilter meshFilter = gameObject.GetOrAdd<MeshFilter>();
            meshFilter.sharedMesh = GetQuadMesh();
        }

        void InitializeMeshRenderer()
        {
            _meshRenderer = gameObject.GetOrAdd<MeshRenderer>();
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        void BuildPanel()
        {
            CreateRenderTexture();
            CreatePanelSettings();
            CreateUIDocument();
            CreateMaterial();

            SetMaterialToRenderer();
            SetPanelSize();
        }

        void CreateRenderTexture()
        {
            RenderTextureDescriptor descriptor = _renderTextureAsset.descriptor;
            descriptor.width = _panelWidth;
            descriptor.height = _panelHeight;
            _renderTexture = new RenderTexture(descriptor) { name = $"{name} - RenderTexture" };
        }

        void CreatePanelSettings()
        {
            _panelSettings = Instantiate(_panelSettingsAsset);
            _panelSettings.targetTexture = _renderTexture;
            _panelSettings.clearColor = true;
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _panelSettings.name = $"{name} - PanelSettings";
        }

        void CreateUIDocument()
        {
            _uiDocument = gameObject.GetOrAdd<UIDocument>();
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;
        }

        void CreateMaterial()
        {
            string shaderName =
                _panelSettings.colorClearValue.a < 1.0f ? k_transparentShader : k_textureShader;
            _material = new Material(Shader.Find(shaderName));
            _material.SetTexture(MainTex, _renderTexture);
        }

        void SetMaterialToRenderer()
        {
            if (_meshRenderer != null)
            {
                _meshRenderer.sharedMaterial = _material;
            }
        }

        void SetPanelSize()
        {
            if (
                _renderTexture != null
                && (_renderTexture.width != _panelWidth || _renderTexture.height != _panelHeight)
            )
            {
                _renderTexture.Release();
                _renderTexture.width = _panelWidth;
                _renderTexture.height = _panelHeight;
                _renderTexture.Create();

                _uiDocument?.rootVisualElement?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(
                _panelWidth / _pixelsPerUnit,
                _panelHeight / _pixelsPerUnit,
                1.0f
            );
        }

        static Mesh GetQuadMesh()
        {
            GameObject tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Mesh quadMesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempQuad);

            return quadMesh;
        }

        void DestroyGeneratedAssets()
        {
            if (_uiDocument)
                Destroy(_uiDocument);
            if (_renderTexture)
                Destroy(_renderTexture);
            if (_panelSettings)
                Destroy(_panelSettings);
            if (_material)
                Destroy(_material);
        }

        void OnDestroy()
        {
            DestroyGeneratedAssets();
        }
    }
}
