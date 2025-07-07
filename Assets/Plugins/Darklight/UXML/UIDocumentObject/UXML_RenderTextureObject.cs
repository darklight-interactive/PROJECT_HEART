using System.Collections;
using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace Darklight.UXML
{
    /// <summary>
    /// This class is used to create a GameObject with a RenderTexture that can be used to render a UXML Element.
    /// </summary>
    public class UXML_RenderTextureObject : UXML_UIDocumentObject, IUnityEditorListener
    {
        private GameObject _quad;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private RenderTexture _renderTexture;
        private RenderTexture _backBuffer;
        private RenderTexture _frontBuffer;
        private Material _materialInstance;

        [SerializeField, Expandable]
        [CreateAsset("NewRenderTexturePreset", "Assets/Resources/Darklight/UXML/")]
        private UXML_RenderTexturePreset _renderTexturePreset;

        [SerializeField]
        private bool _destroyOnEditorReload = true;

        void Start()
        {
            Initialize();
        }

        protected override void OnEditorReloaded()
        {
#if UNITY_EDITOR
            if (_destroyOnEditorReload)
                DestroyImmediate(this.gameObject);
#endif
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public override void Initialize()
        {
            Initialize(
                Preset,
                new Material(_renderTexturePreset.material),
                new RenderTexture(_renderTexturePreset.renderTexture),
                true
            );
        }

        public void Initialize(
            UXML_UIDocumentPreset preset,
            Material material,
            RenderTexture renderTexture,
            bool clonePanelSettings = false
        )
        {
            _material = material;
            _renderTexture = renderTexture;
            base.Initialize(preset, clonePanelSettings);

            // Create a quad mesh child
            if (_quad == null || _meshRenderer == null)
            {
                // Destroy all children
                foreach (Transform child in this.transform)
                {
                    DestroyImmediate(child.gameObject);
                }

                // Create a new quad
                _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _quad.transform.SetParent(this.transform);
                _quad.transform.localPosition = Vector3.zero;
                _meshRenderer = _quad.GetComponent<MeshRenderer>();

                gameObject.layer = LayerMask.NameToLayer("UI");
                _quad.layer = LayerMask.NameToLayer("UI");
            }

            // Initialize front and back buffers
            if (_renderTexture == null)
                return;

            // Pick a safe format for this platform
            var safeFormat = GetSupportedRenderTextureFormat();
            _backBuffer = new RenderTexture(_renderTexture)
            {
                format = safeFormat,
                enableRandomWrite = false
            };
            _frontBuffer = new RenderTexture(_renderTexture)
            {
                format = safeFormat,
                enableRandomWrite = false
            };

            // Create a new material instance
            _materialInstance = new Material(_material);

            // Assign the front buffer to the panel settings and material initially
            Document.panelSettings.targetTexture = _frontBuffer;
            _materialInstance.mainTexture = _frontBuffer;

            // Assign the material to the mesh renderer
            _meshRenderer.sharedMaterial = _materialInstance;
        }

        public void FixedUpdate()
        {
            // Only call TextureUpdate if necessary
            if (Root.resolvedStyle.width > 0 && Root.resolvedStyle.height > 0)
            {
                TextureUpdate();
            }
        }

        void TextureUpdate()
        {
            StartCoroutine(TextureUpdateRoutine());
        }

        RenderTextureFormat GetSupportedRenderTextureFormat()
        {
            // Preferred order of formats (higher quality to lower)
            RenderTextureFormat[] preferredFormats = new[]
            {
                RenderTextureFormat.ARGBHalf,
                RenderTextureFormat.DefaultHDR,
                RenderTextureFormat.Default,
                RenderTextureFormat.ARGB32
            };

            foreach (var format in preferredFormats)
            {
                if (SystemInfo.SupportsRenderTextureFormat(format))
                    return format;
            }

            // Final fallback
            return RenderTextureFormat.ARGB32;
        }

        IEnumerator TextureUpdateRoutine()
        {
            if (_meshRenderer == null)
            {
                Debug.LogWarning("Mesh renderer is not initialized.", this);
                yield break;
            }

            // Render to the back buffer
            RenderToBackBuffer();

            yield return new WaitForEndOfFrame();

            // Swap buffers
            SwapBuffers();

            // Update the material to use the new front buffer
            _meshRenderer.sharedMaterial.mainTexture = _frontBuffer;

            // Update the panel settings with the new front buffer
            Document.panelSettings.targetTexture = _frontBuffer;

            // Force the UI document to repaint
            Document.rootVisualElement.MarkDirtyRepaint();
        }

        private void RenderToBackBuffer()
        {
            if (_backBuffer == null)
            {
                Debug.LogWarning("Back buffer is not initialized.");
                return;
            }

            // Set the back buffer as the target for rendering
            Document.panelSettings.targetTexture = _backBuffer;

            // Clear the back buffer
            RenderTexture.active = _backBuffer;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            // Force the UI document to repaint and render onto the back buffer
            Document.rootVisualElement.MarkDirtyRepaint();

            // Ensure the UI rendering occurs
            //UIElementsUtility.UpdatePanels();
        }

        private void SwapBuffers()
        {
            // Swap the front and back buffers
            var temp = _frontBuffer;
            _frontBuffer = _backBuffer;
            _backBuffer = temp;
        }

        public void SetLocalScale(float scale)
        {
            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Destroy()
        {
            if (Application.isPlaying)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            CustomGizmos.DrawWireRect(
                this.transform.position,
                this.transform.localScale,
                Vector3.forward,
                Color.white
            );
        }
#endif
    }
}
