using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    [CreateAssetMenu(menuName = "Darklight/UXML/RenderTexturePreset")]
    public class UXML_RenderTexturePreset : ScriptableObject
    {
        public Material material;
        public RenderTexture renderTexture;
    }
}
