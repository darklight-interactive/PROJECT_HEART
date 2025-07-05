using System;
using Darklight.Input;
using Darklight.World;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RemixSurvivors.Survivor
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public partial class SurvivorMovementController : UniversalInputController
    {
        // -- ( Constants ) -------------------------------------------------------------------------------------------- >>
        const string PREFIX = "<color=green>[STRPlayerController]</color> ";

        // -- ( Fields ) -------------------------------------------------------------------------------------------- >>
        Rigidbody _rb;
        MotionVector _mv_targetDirection = MotionVector.Zero;
        MotionVector _mv_currentDirection = MotionVector.Zero;
        MotionVector _mv_targetVelocity = MotionVector.Zero;
        MotionVector _mv_currentVelocity = MotionVector.Zero;
        Quaternion _mv_currentRotation = Quaternion.identity;
        Quaternion _mv_targetRotation = Quaternion.identity;

        // -- ( Serialized Fields ) -------------------------------------------------------------------------------------------- >>
        [Header("References")]
        [SerializeField, Required]
        Transform _modelTransform;

        [Header("Active Data")]
        [SerializeField]
        SurvivorMovementStateMachine _stateMachine;

        [SerializeField]
        Sensor _groundSensor;

        [SerializeField]
        Sensor _grindableSensor;

        [Header("Settings")]
        [SerializeField, Required, Expandable]
        SurvivorMovementSettings _settings;

        // -- ( Properties ) -------------------------------------------------------------------------------------------- >>
        public SurvivorMovementSettings Settings => _settings;
        public SurvivorMovementStateMachine StateMachine => _stateMachine;
        public Sensor GroundSensor => _groundSensor;
        public Sensor GrindableSensor => _grindableSensor;
        public Vector3 CurrentPosition => transform.position;
        public MotionVector TargetDirection => _mv_targetDirection;
        public MotionVector CurrentDirection => _mv_currentDirection;
        public MotionVector TargetVelocity => _mv_targetVelocity;
        public MotionVector CurrentVelocity => _mv_currentVelocity;
        public Quaternion TargetRotation => _mv_targetRotation;
        public Quaternion CurrentRotation => _mv_currentRotation;

        // -- ( Events ) -------------------------------------------------------------------------------------------- >>
        public Action OnFixedUpdateEvent;
        public Action OnDrawGizmosEvent;
        public Action OnDrawGizmosSelectedEvent;

        #region < PRIVATE_METHODS > [[ Unity Methods ]] ================================================================
        void Start()
        {
            // (( Rigidbody )) ----------------- ))
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

            // (( Ground Check )) ----------------- ))
            _groundSensor = new Sensor(Settings.GroundSensorSettings, this);
            OnFixedUpdateEvent += _groundSensor.Execute; // << execute the ground sensor on fixed update

            // (( Grindable Check ))
            _grindableSensor = new Sensor(Settings.GrindableSensorSettings, this);
            OnFixedUpdateEvent += _grindableSensor.Execute; // << execute the grindable sensor on fixed update

            // (( StateMachine )) ----------------- ))
            _stateMachine = new SurvivorMovementStateMachine(this);
            _stateMachine.GoToState(MovementState.IDLE);
            OnFixedUpdateEvent += _stateMachine.Step; // << step the state machine on fixed update

            // (( Set Start Values )) ----------------- ))
            _mv_currentRotation = _modelTransform.rotation;
            _mv_targetRotation = _mv_currentRotation;
        }

        void FixedUpdate()
        {
            // Invoke the fixed update event
            OnFixedUpdateEvent?.Invoke();

            // Clamp the velocity
            _mv_currentVelocity.Clamp(Settings.MaxHorizontalVelocity, Settings.MaxVerticalVelocity);

            // Assign velocity to rigidbody
            _rb.linearVelocity = _mv_currentVelocity.Combined;

            // Assign rotation to model
            _modelTransform.rotation = _mv_currentRotation;
        }

        void OnDrawGizmos()
        {
            // Invoke the draw gizmos event
            OnDrawGizmosEvent?.Invoke();

            // Draw the sensor gizmos
            _groundSensor.DrawGizmos();
            _grindableSensor.DrawGizmos();
        }

        void OnDrawGizmosSelected()
        {
            // Invoke the draw gizmos selected event
            OnDrawGizmosSelectedEvent?.Invoke();
        }

        void OnDisable()
        {
            OnFixedUpdateEvent = null;
            OnDrawGizmosEvent = null;
            OnDrawGizmosSelectedEvent = null;
        }

        void OnDestroy()
        {
            OnFixedUpdateEvent = null;
            OnDrawGizmosEvent = null;
            OnDrawGizmosSelectedEvent = null;
        }

        #endregion

        #region < PRIVATE_METHODS > [[ Calculate Target Values ]] ================================================================
        void CalculateAndSetTargetHorizontalDirection(ref MotionVector targetDirection)
        {
            float rotatedX = MoveInput.x - MoveInput.y;
            float rotatedZ = MoveInput.x + MoveInput.y;
            targetDirection.Horizontal = new Vector2(rotatedX, rotatedZ).normalized;
        }

        void CalculateAndSetTargetHorizontalVelocity(
            Vector2 targetDirection,
            ref MotionVector targetVelocity
        )
        {
            targetVelocity.Horizontal = targetDirection * Settings.MovementVelocity;
        }

        void CalculateAndSetTargetVelocityToPosition(
            Vector3 targetPosition,
            float forceMultiplier,
            ref MotionVector targetVelocity
        )
        {
            targetVelocity.Combined = (targetPosition - transform.position) * forceMultiplier;
        }

        void CalculateAndSetTargetRotation(MotionVector direction, ref Quaternion targetRotation)
        {
            Vector3 vec3Direction = new Vector3(direction.Horizontal.x, 0f, direction.Horizontal.y);
            targetRotation = Quaternion.LookRotation(vec3Direction, Vector3.up);
        }

        #endregion

        #region < PRIVATE_METHODS > [[ Update Current Values ]] ================================================================
        /// <summary>
        /// Updates the current direction to the target direction.
        /// </summary>
        /// <param name="targetHorizontalDirection">The target direction.</param>
        /// <param name="currentHorizontalDirection">The current direction.</param>
        void UpdateCurrentHorizontalDirection(
            Vector2 targetHorizontalDirection,
            ref MotionVector currentHorizontalDirection,
            float accelMultiplier = 1f
        )
        {
            currentHorizontalDirection.Horizontal = Vector2.Lerp(
                currentHorizontalDirection.Horizontal,
                targetHorizontalDirection,
                accelMultiplier * Time.deltaTime
            );
        }

        /// <summary>
        /// Updates the current velocity, lerping to the target velocity.
        /// </summary>
        /// <param name="targetVelocity">The target velocity.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="accelMultiplier">The acceleration multiplier.</param>
        void UpdateCurrentVelocity(
            MotionVector targetVelocity,
            ref MotionVector currentVelocity,
            float accelMultiplier = 1
        )
        {
            currentVelocity = MotionVector.Lerp(
                currentVelocity,
                targetVelocity,
                accelMultiplier * Time.deltaTime
            );
        }

        /// <summary>
        /// Updates the current horizontal velocity to the target horizontal velocity.
        /// </summary>
        /// <param name="targetHorzVelocity">The target horizontal velocity.</param>
        /// <param name="currentHorzVelocity">The current horizontal velocity.</param>
        /// <param name="accelMultiplier">The acceleration multiplier.</param>
        void UpdateCurrentHorizontalVelocity(
            Vector2 targetHorzVelocity,
            ref MotionVector currentHorzVelocity,
            float accelMultiplier = 1f
        )
        {
            currentHorzVelocity.Horizontal = Vector2.Lerp(
                currentHorzVelocity.Horizontal,
                targetHorzVelocity,
                accelMultiplier * Time.deltaTime
            );
        }

        /// <summary>
        /// Updates the current vertical velocity to the target vertical velocity.
        /// </summary>
        /// /// <param name="targetVertVelocity">The target vertical velocity.</param>
        /// <param name="currentVertVelocity">The current vertical velocity.</param>
        /// <param name="accelMultiplier">The acceleration multiplier.</param>
        void UpdateCurrentVerticalVelocity(
            float targetVertVelocity,
            ref MotionVector currentVertVelocity,
            float accelMultiplier = 1f
        )
        {
            currentVertVelocity.Vertical = Mathf.Lerp(
                currentVertVelocity.Vertical,
                targetVertVelocity,
                accelMultiplier * Time.deltaTime
            );
        }

        /// <summary>
        /// Updates the current rotation to the target rotation.
        /// </summary>
        /// <param name="targetRotation">The target rotation.</param>
        /// <param name="currentRotation">The current rotation.</param>
        /// <param name="accelMultiplier">The acceleration multiplier.</param>
        void UpdateCurrentRotation(
            Quaternion targetRotation,
            ref Quaternion currentRotation,
            float accelMultiplier = 1f
        )
        {
            currentRotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                accelMultiplier * Time.deltaTime
            );
        }
        #endregion

        #region < PUBLIC_METHODS > [[ Initialize ]] ================================================================

        public void Initialize()
        {
            this.Start();
        }

        #endregion

        #region < PUBLIC_METHODS > [[ Setters ]] ================================================================
        public void SetTargetRotation(Quaternion targetRotation) =>
            _mv_targetRotation = targetRotation;

        #endregion

        #region < PUBLIC_METHODS > [[ Fixed Update Execution Methods ]] ================================================================

        /// <summary>
        /// Updates the horizontal movement acceleration while skating on the ground.
        /// Calculates the target direction and velocity from inputs, then updates the current direction and velocity.
        /// </summary>
        public void ExecuteHorizontalMovement_GroundSkateAcceleration()
        {
            // << CALCULATE & UPDATE DIRECTION >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetHorizontalDirection(ref _mv_targetDirection);
            UpdateCurrentHorizontalDirection(
                _mv_targetDirection.Horizontal,
                ref _mv_currentDirection
            );

            // << CALCULATE & UPDATE HORIZONTAL VELOCITY >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetHorizontalVelocity(
                _mv_targetDirection.Horizontal,
                ref _mv_targetVelocity
            );
            UpdateCurrentHorizontalVelocity(
                _mv_targetVelocity.Horizontal,
                ref _mv_currentVelocity,
                Settings.MovementAcceleration
            );

            // << CALCULATE & UPDATE ROTATION >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetRotation(_mv_currentDirection, ref _mv_targetRotation);
            UpdateCurrentRotation(
                _mv_targetRotation,
                ref _mv_currentRotation,
                Settings.RotationAcceleration
            );
        }

        /// <summary>
        /// Updates the horizontal movement deceleration while skating on the ground.
        /// Resets the target horizontal velocity to zero, then updates the current horizontal velocity.
        /// Does not change the direction.
        /// </summary>
        public void ExecuteHorizontalMovement_GroundSkateDeceleration()
        {
            // << RESET TARGET HORIZONTAL VELOCITY >> -------------------------------------------------------------------------------------------- >>
            _mv_targetVelocity.Horizontal = Vector3.zero;
            UpdateCurrentHorizontalVelocity(
                _mv_targetVelocity.Horizontal,
                ref _mv_currentVelocity,
                Settings.MovementDeceleration
            );

            // << UPDATE ROTATION * DECELERATION >> -------------------------------------------------------------------------------------------- >>
            UpdateCurrentRotation(
                _mv_targetRotation,
                ref _mv_currentRotation,
                Settings.RotationDeceleration
            );
        }

        public void ExecuteHorizontalMovement_InAirDamping()
        {
            // << CALCULATE & UPDATE DIRECTION >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetHorizontalDirection(ref _mv_targetDirection);
            UpdateCurrentHorizontalDirection(
                _mv_targetDirection.Horizontal,
                ref _mv_currentDirection
            );

            // << CALCULATE & UPDATE HORIZONTAL VELOCITY >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetHorizontalVelocity(
                _mv_targetDirection.Horizontal,
                ref _mv_targetVelocity
            );

            // Apply the dampling value
            UpdateCurrentHorizontalVelocity(
                _mv_targetVelocity.Horizontal,
                ref _mv_currentVelocity,
                Settings.AirMovementDamping
            );

            // << CALCULATE & UPDATE ROTATION >> -------------------------------------------------------------------------------------------- >>
            CalculateAndSetTargetRotation(_mv_currentDirection, ref _mv_targetRotation);
            UpdateCurrentRotation(
                _mv_targetRotation,
                ref _mv_currentRotation,
                Settings.AirRotationDamping
            );
        }

        /// <summary>
        /// Updates the vertical movement acceleration while jumping.
        /// </summary>
        public void ExecuteVerticalMovement_JumpInitialBoost(
            ref float jumpElapsedTime,
            out bool isJumpTimeComplete
        )
        {
            // Apply smooth step to the vertical velocity
            float verticalVelocity = Mathf.SmoothStep(
                Settings.JumpVelocity,
                Settings.JumpVelocity * 0.30f,
                jumpElapsedTime / Settings.JumpBoostDuration
            );

            // Subtract gravity from the vertical velocity
            verticalVelocity -= Settings.Gravity * Time.fixedDeltaTime;

            // Update the current vertical velocity
            _mv_targetVelocity.Vertical = _mv_currentVelocity.Vertical = verticalVelocity;

            // Increases the time that has passed since the player started the jump
            jumpElapsedTime += Time.fixedDeltaTime;
            isJumpTimeComplete = jumpElapsedTime >= Settings.JumpBoostDuration;
        }

        public void ExecuteVerticalMovement_GravityConstant()
        {
            float verticalVelocity = Mathf.Max(
                _mv_currentVelocity.Vertical - (Settings.Gravity * Time.fixedDeltaTime),
                -Settings.MaxVerticalVelocity
            );
            _mv_targetVelocity.Vertical = _mv_currentVelocity.Vertical = verticalVelocity;
        }

        /// <summary>
        /// Executes rail grinding movement using velocity and forces
        /// </summary>
        /// <param name="currentPoint">Current point on the rail</param>
        /// <param name="nextPoint">Next point on the rail for direction</param>
        /// <param name="speed">Speed of movement along rail</param>
        public void ExecuteFullVelocityToPosition(
            Vector3 targetPosition,
            float forceMultiplier = 1f
        )
        {
            CalculateAndSetTargetVelocityToPosition(
                targetPosition,
                forceMultiplier,
                ref _mv_targetVelocity
            );
            UpdateCurrentVelocity(
                _mv_targetVelocity,
                ref _mv_currentVelocity,
                Settings.MovementAcceleration
            );
        }
        #endregion
    }
}
