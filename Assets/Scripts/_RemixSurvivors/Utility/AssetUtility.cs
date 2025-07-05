using Darklight.Utility;
using UnityEditor;
using UnityEngine;

namespace RemixSurvivors
{
    public static class AssetUtility
    {
        const string PATH = "Assets/Resources/RemixSurvivors";

        public static T CreateOrLoadScriptableObject<T>(string name)
            where T : ScriptableObject
        {
            var asset = ScriptableObjectUtility.CreateOrLoadScriptableObject<T>(PATH, name);
            return asset;
        }
    }
}
