using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    public static class UXML_Utility
    {
        /// <summary>
        /// Creates a new UIDocumentObject from a given preset.
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public static TDocument CreateUIDocumentObject<TDocument>(UXML_UIDocumentPreset preset)
            where TDocument : UXML_UIDocumentObject
        {
            GameObject go = new GameObject($"UXML_UIDocument : {preset.name}");
            //go.hideFlags = HideFlags.NotEditable;
            TDocument uiDocument = go.AddComponent<TDocument>();
            uiDocument.Initialize(preset);
            return uiDocument;
        }

        /// <summary>
        /// Sets the position of a UI Toolkit element to correspond to a world position.
        /// Optionally centers the element on the screen position.
        /// </summary>
        /// <param name="element">The UI Toolkit element to position.</param>
        /// <param name="worldPosition">The world position to map to screen space.</param>
        /// <param name="center">Optional parameter to center the element at the screen position (default false).</param>
        public static void SetWorldToScreenPoint(
            VisualElement element,
            Vector3 worldPosition,
            bool center = false
        )
        {
            Camera cam = Camera.main;
            if (cam == null)
                throw new System.Exception("No main camera found.");

            // Convert world position to screen position
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
            screenPosition.y = cam.pixelHeight - screenPosition.y; // UI Toolkit uses top-left origin
            screenPosition.z = 0;

            if (center)
            {
                // Adjust position to center the element
                screenPosition.x -= element.resolvedStyle.width / 2;
                screenPosition.y -= element.resolvedStyle.height / 2;
            }

            // Set positions using left and top in style
            element.style.left = screenPosition.x;
            element.style.top = screenPosition.y;

            Debug.Log($"Set Element Position: {screenPosition}");
        }

        /// <summary>
        /// Creates a new GameObject with a UXML_RenderTextureObject component.
        /// This allows for the rendering of a UXML Element to a In-World RenderTexture.
        /// </summary>
        /// <param name="preset">
        ///     The UXML_UIDocumentPreset to use for the RenderTextureObject.
        /// </param>
        /// <returns></returns>
        ///
        public static T CreateUXMLRenderTextureObject<T>(
            UXML_UIDocumentPreset preset,
            RenderTexture renderTextureAsset
        )
            where T : UXML_RenderTextureObject
        {
            string name = $"UXMLRenderTexture : unknown";
            if (preset != null)
                name = $"UXMLRenderTexture : {preset.name}";
            GameObject go = new GameObject(name);

            //go.hideFlags = HideFlags.NotEditable;
            T renderTextureObject = go.AddComponent<T>();
            renderTextureObject.Initialize(preset);
            return renderTextureObject;
        }
    }
}
