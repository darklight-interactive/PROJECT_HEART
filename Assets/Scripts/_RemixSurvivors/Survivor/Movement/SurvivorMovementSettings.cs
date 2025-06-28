using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using RemixSurvivors.AI;
using UnityEngine;

namespace RemixSurvivors.Survivor
{
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "SurvivorMovementSettings",
        menuName = "Survivor/Movement/SurvivorMovementSettings"
    )]
    public class SurvivorMovementSettings : ScriptableObject
    {
        [Header("World")]
        [SerializeField, ReadOnly, AllowNesting]
        float _gravity = 9.8f;

        [Header("Velocity Limits")]
        [SerializeField, Range(1, 100)]
        float _maxHorizontalVelocity = 30f;

        [SerializeField, Range(1, 100)]
        float _maxVerticalVelocity = 10f;

        [Header("Movement")]
        [SerializeField, Range(1, 10)]
        float _movementAcceleration = 2f;

        [SerializeField, Range(1, 10)]
        float _movementDeceleration = 2f;

        [SerializeField, Range(1, 100)]
        float _movementVelocity = 10f;

        [Header("Rotation")]
        [SerializeField, Range(1, 100)]
        float _rotationAcceleration = 10f;

        [SerializeField, Range(1, 100)]
        float _rotationDeceleration = 10f;

        [Header("Jump")]
        [SerializeField, Range(1, 400)]
        float _jumpVelocity = 18f;

        [SerializeField, Range(0, 1)]
        float _jumpBoostDuration = 0.85f;

        [Header("Air Control")]
        [SerializeField, Range(0, 1)]
        float _airMovementDamping = 0.5f;

        [SerializeField, Range(0, 1)]
        float _airRotationDamping = 0.5f;

        [Header("Sensor Settings")]
        [SerializeField, Expandable, AllowNesting]
        SensorSettings _groundSensorSettings;

        [SerializeField, Expandable, AllowNesting]
        SensorSettings _grindableSensorSettings;

        [Header("Rail Movement")]
        [SerializeField, Range(1, 100)]
        private float _grindSpeed = 15f;

        [SerializeField, Range(1, 100)]
        private float _grindAcceleration = 5f;

        [SerializeField, Range(1, 100)]
        private float _grindDeceleration = 3f;

        [SerializeField, Range(1, 100)]
        private float _grindRotationSpeed = 10f;

        [SerializeField, Range(1, 100)]
        private float _railSpringForce = 50f;

        [SerializeField, Range(1, 100)]
        private float _railDampingForce = 5f;

        [SerializeField, Range(1, 100)]
        private float _railSnapSpeed = 20f;

        [SerializeField, Range(1, 100)]
        private float _railExitForce = 15f;

        [SerializeField, Range(1, 100)]
        private float _railExitUpwardForce = 5f;

        public float Gravity => _gravity;
        public SensorSettings GroundSensorSettings
        {
            get
            {
                if (_groundSensorSettings == null)
                {
                    _groundSensorSettings =
                        AssetUtility.CreateOrLoadScriptableObject<SensorSettings>(
                            "GroundSensorSettings"
                        );
                }
                return _groundSensorSettings;
            }
            set { _groundSensorSettings = value; }
        }
        public SensorSettings GrindableSensorSettings
        {
            get
            {
                if (_grindableSensorSettings == null)
                {
                    _grindableSensorSettings =
                        AssetUtility.CreateOrLoadScriptableObject<SensorSettings>(
                            "GrindableSensorSettings"
                        );
                }
                return _grindableSensorSettings;
            }
            set { _grindableSensorSettings = value; }
        }
        public float MaxHorizontalVelocity => _maxHorizontalVelocity;
        public float MaxVerticalVelocity => _maxVerticalVelocity;
        public float MovementAcceleration => _movementAcceleration;
        public float MovementDeceleration => _movementDeceleration;
        public float MovementVelocity => _movementVelocity;
        public float RotationAcceleration => _rotationAcceleration;
        public float RotationDeceleration => _rotationDeceleration;
        public float JumpVelocity => _jumpVelocity;
        public float JumpBoostDuration => _jumpBoostDuration;
        public float AirMovementDamping => _airMovementDamping;
        public float AirRotationDamping => _airRotationDamping;
        public float GrindSpeed => _grindSpeed;
        public float GrindAcceleration => _grindAcceleration;
        public float GrindDeceleration => _grindDeceleration;
        public float GrindRotationSpeed => _grindRotationSpeed;
        public float RailSnapSpeed => _railSnapSpeed;
        public float RailExitForce => _railExitForce;
        public float RailExitUpwardForce => _railExitUpwardForce;
        public float RailSpringForce => _railSpringForce;
        public float RailDampingForce => _railDampingForce;
    }
}
