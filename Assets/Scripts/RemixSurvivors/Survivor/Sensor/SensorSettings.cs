using UnityEngine;

namespace RemixSurvivors.Survivor
{
    /// <summary>
    /// ScriptableObject containing all sensor-related settings for the survivor.
    /// </summary>
    [CreateAssetMenu(fileName = "SensorSettings", menuName = "RemixSurvivors/Settings/SensorSettings")]
    public class SensorSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Radius of the detection sphere")]
        [Range(0.01f, 25f)]
        private float _radius = 0.2f;

        [SerializeField]
        [Tooltip("Layer mask for ground detection")]
        private LayerMask _layer;


        [Header("Debug")]
        [SerializeField]
        [Tooltip("Enable gizmo visualization in editor")]
        private bool _showDebugGizmos = true;

        [SerializeField, NaughtyAttributes.ShowIf("_showDebugGizmos")]
        [Tooltip("Color of the gizmo when the sensor is colliding")]
        private Color _collidingColor = Color.green;

        public float Radius => _radius;
        public LayerMask Layer => _layer;
        public bool ShowDebugGizmos => _showDebugGizmos;
        public Color DebugCollidingColor => _collidingColor;

        /// <summary>
        /// Validates the settings to ensure they are within acceptable ranges.
        /// </summary>
        private void OnValidate()
        {
            _radius = Mathf.Max(0.01f, _radius);
        }
    }
} 