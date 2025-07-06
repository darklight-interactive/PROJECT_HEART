using System;
using System.Collections.Generic;
using Darklight.Behaviour;
using Darklight.Editor;
using Darklight.World;
using NaughtyAttributes;
using ProjectHeart.Input;
using UnityEngine;

namespace ProjectHeart.Character
{
    public enum MovementState
    {
        /// <summary> The character is not moving. </summary>
        IDLE,

        /// <summary> The character is moving on the ground. </summary>
        GROUND_MOVE,

        /// <summary> The character is jumping while on the ground. </summary>
        GROUND_JUMP,

        /// <summary> The character is falling. </summary>
        FALLING
    }

    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public partial class CharacterMovementController : MonoBehaviour
    {
        // -- ( Constants ) -------------------------------------------------------------------------------------------- >>
        const string PREFIX = "<color=green>[CharacterMovementController]</color> ";

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

        [SerializeField, Required]
        Camera _playerCamera;

        [Header("Active Data")]
        [SerializeField]
        MovementStateMachine _stateMachine;

        [SerializeField]
        Sensor _groundSensor;

        [Header("Settings")]
        [SerializeField, Required, Expandable]
        CharacterMovementSettings _settings;

        // -- ( Properties ) -------------------------------------------------------------------------------------------- >>
        public MovementStateMachine StateMachine => _stateMachine;
        public CharacterMovementSettings Settings => _settings;
        public ISensor GroundSensor => _groundSensor;
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

            // (( StateMachine )) ----------------- ))
            _stateMachine = new MovementStateMachine(this);
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
        }

        void OnDrawGizmosSelected()
        {
            // Invoke the draw gizmos selected event
            OnDrawGizmosSelectedEvent?.Invoke();

            // Draw the movement gizmos
            DrawMovementGizmos();
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
        /// <summary>
        /// Calculates and sets the target horizontal direction based on camera-relative input.
        /// Transforms the input direction from camera space to world space.
        /// </summary>
        /// <param name="targetDirection">The target direction to be updated.</param>
        void CalculateAndSetTargetHorizontalDirection(ref MotionVector targetDirection)
        {
            Vector2 moveInput = GlobalInputReader.PlayerInput.MoveValue;

            // Get camera's forward and right vectors, ignoring Y component
            Vector3 cameraForward = _playerCamera.transform.forward;
            Vector3 cameraRight = _playerCamera.transform.right;

            // Flatten vectors to horizontal plane
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize the flattened vectors
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Transform input from camera space to world space
            Vector3 worldDirection = (cameraRight * moveInput.x) + (cameraForward * moveInput.y);

            // Convert to 2D horizontal direction
            targetDirection.Horizontal = new Vector2(worldDirection.x, worldDirection.z).normalized;
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

        #region < PRIVATE_METHODS > [[ Draw Custom Gizmos ]] ================================================================
        void DrawMovementGizmos()
        {
            Vector3 position = transform.position;

            // === Draw Directions ===
            Vector3 currentDir = new Vector3(
                _mv_currentDirection.Horizontal.x,
                0f,
                _mv_currentDirection.Horizontal.y
            );
            Vector3 targetDir = new Vector3(
                _mv_targetDirection.Horizontal.x,
                0f,
                _mv_targetDirection.Horizontal.y
            );

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + currentDir * 2f);
            Gizmos.DrawSphere(position + currentDir * 2f, 0.05f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(position, position + targetDir * 2f);
            Gizmos.DrawWireSphere(position + targetDir * 2f, 0.05f);

            // === Draw Velocities ===
            Vector3 currentVel = _mv_currentVelocity.Combined;
            Vector3 targetVel = _mv_targetVelocity.Combined;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + currentVel * 0.5f);
            Gizmos.DrawCube(position + currentVel * 0.5f, Vector3.one * 0.05f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + targetVel * 0.5f);
            Gizmos.DrawWireCube(position + targetVel * 0.5f, Vector3.one * 0.05f);

            // === Draw Rotations ===
            Vector3 forward = _modelTransform.forward;

            // Current Rotation (red)
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, _mv_currentRotation * Vector3.forward * 1.5f);

            // Target Rotation (magenta)
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(position, _mv_targetRotation * Vector3.forward * 1.5f);
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
        /// Updates the horizontal movement acceleration while moving on the ground.
        /// Calculates the target direction and velocity from inputs, then updates the current direction and velocity.
        /// </summary>
        public void ExecuteHorizontalMovement_GroundMoveAcceleration()
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
        /// Updates the horizontal movement deceleration while moving on the ground.
        /// Resets the target horizontal velocity to zero, then updates the current horizontal velocity.
        /// Does not change the direction.
        /// </summary>
        public void ExecuteHorizontalMovement_GroundMoveDeceleration()
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

        [System.Serializable]
        public class MovementStateMachine : FiniteStateMachine<MovementState>
        {
            CharacterMovementController _controller;
            public CharacterMovementController Controller => _controller;

            public MovementStateMachine(CharacterMovementController controller)
                : base(MovementState.IDLE)
            {
                _controller = controller;
                OverrideFiniteStates(
                    new Dictionary<MovementState, FiniteState<MovementState>>()
                    {
                        { MovementState.IDLE, new IdleState(this) },
                        { MovementState.GROUND_MOVE, new GroundMoveState(this) },
                        { MovementState.GROUND_JUMP, new GroundJumpState(this) },
                        { MovementState.FALLING, new FallingState(this) },
                    }
                );
            }
        }
    }
}
