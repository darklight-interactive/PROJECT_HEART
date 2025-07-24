using System.Collections;
using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityUtils;

namespace Darklight.UXML
{
    /// <summary>
    /// This class is used to create a GameObject with a RenderTexture that can be used to render a UXML Element.
    /// </summary>
    public class UXML_RenderTextureObject : UXML_UIDocumentObject, IUnityEditorListener
    {
        const string k_transparentShader = "Unlit/Transparent";
        const string k_textureShader = "Unlit/Texture";
        const string k_mainTex = "_MainTex";
        static readonly int MainTex = Shader.PropertyToID(k_mainTex);

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;

        Material _material;
        RenderTexture _renderTexture;
        RenderTexture _backBuffer;
        RenderTexture _frontBuffer;

        [SerializeField, Required, Expandable]
        [CreateAsset(
            "NewRenderTextureObjectSettings",
            "Assets/Resources/Darklight/UXML/UIDocumentObject/RenderTextureObject"
        )]
        UXML_RenderTextureObjectSettings _settings;

        public override void Initialize()
        {
            // < INITIALIZE BASE > //
            base.Initialize();

            // < INITIALIZE COMPONENTS > //
            InitializeMeshRenderer();
            _meshFilter = gameObject.GetOrAdd<MeshFilter>();
            if (_meshFilter.sharedMesh == null)
                _meshFilter.sharedMesh = GetQuadMesh();

            // < CREATE RENDER TEXTURE > //
            CreateRenderTexture(out _renderTexture);
            CreateMaterial(out _material);

            // < SET VALUES > //
            SetPanelSettings();
            SetMaterialToRenderer();
            SetPanelSize();

            /* !DOUBLE BUFFERING !
            // Initialize front and back buffers
            CreateRenderTexture(out _backBuffer);
            CreateRenderTexture(out _frontBuffer);
            */
        }

        /// <summary>
        /// Initialize the mesh renderer.
        /// </summary>
        void InitializeMeshRenderer()
        {
            _meshRenderer = this.gameObject.GetOrAdd<MeshRenderer>();
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        /// <summary>
        /// Create a new render texture with the specified width and height and apply the supported format.
        /// </summary>
        void CreateRenderTexture(out RenderTexture renderTexture)
        {
            RenderTextureDescriptor descriptor = _settings.renderTextureAsset.descriptor;
            descriptor.width = _settings.panelWidth;
            descriptor.height = _settings.panelHeight;

            renderTexture = new RenderTexture(descriptor)
            {
                name = $"{name} - RenderTexture",
                format = GetSupportedRenderTextureFormat(),
                enableRandomWrite = false
            };
        }

        /// <summary>
        /// Create a new material with the specified shader and set the main texture to the render texture.
        /// </summary>
        void CreateMaterial(out Material material)
        {
            string shaderName =
                PanelSettings.colorClearValue.a < 1.0f ? k_transparentShader : k_textureShader;
            material = new Material(Shader.Find(shaderName));
            material.SetTexture(MainTex, _renderTexture);
        }

        /// <summary>
        /// Apply unique settings to the panel settings asset. Set the target texture to the render texture.
        /// </summary>
        void SetPanelSettings()
        {
            PanelSettings.targetTexture = _renderTexture;
            PanelSettings.clearColor = true;
            PanelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            PanelSettings.scale = _settings.panelScale;
        }

        /// <summary>
        /// Set the material to the mesh renderer.
        /// </summary>
        void SetMaterialToRenderer()
        {
            if (_meshRenderer != null)
            {
                _meshRenderer.sharedMaterial = _material;
            }
        }

        /// <summary>
        /// Set the size of the panel based on the panelWidth, panelHeight, and pixelsPerUnit.
        /// </summary>
        void SetPanelSize()
        {
            if (
                _renderTexture != null
                && (
                    _renderTexture.width != _settings.panelWidth
                    || _renderTexture.height != _settings.panelHeight
                )
            )
            {
                _renderTexture.Release();
                _renderTexture.width = _settings.panelWidth;
                _renderTexture.height = _settings.panelHeight;
                _renderTexture.Create();

                Root?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(
                _settings.panelWidth / _settings.pixelsPerUnit,
                _settings.panelHeight / _settings.pixelsPerUnit,
                1.0f
            );
        }

        /* !DOUBLE BUFFERING !
        void FixedUpdate()
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
            PanelSettings.targetTexture = _frontBuffer;

            // Force the UI document to repaint
            Root.MarkDirtyRepaint();
        }

        void RenderToBackBuffer()
        {
            if (_backBuffer == null)
            {
                Debug.LogWarning("Back buffer is not initialized.");
                return;
            }

            // Set the back buffer as the target for rendering
            PanelSettings.targetTexture = _backBuffer;

            // Clear the back buffer
            RenderTexture.active = _backBuffer;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            // Force the UI document to repaint and render onto the back buffer
            Root.MarkDirtyRepaint();

            // Ensure the UI rendering occurs
            //UIElementsUtility.UpdatePanels();
        }

        void SwapBuffers()
        {
            // Swap the front and back buffers
            var temp = _frontBuffer;
            _frontBuffer = _backBuffer;
            _backBuffer = temp;
        }
        */

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

        static Mesh GetQuadMesh()
        {
            GameObject tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Mesh quadMesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;

            if (Application.isPlaying)
                Destroy(tempQuad);
            else
                DestroyImmediate(tempQuad);

            return quadMesh;
        }

        static RenderTextureFormat GetSupportedRenderTextureFormat()
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
