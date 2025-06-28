using System.Collections;
using Darklight.UnityExt.Editor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RemixSurvivors.Survivor
{
    /// <summary>
    /// Handles the state when a survivor is grinding on a rail.
    /// </summary>
    [System.Serializable]
    public class GrindState : BaseState
    {
        private const int FORWARD = 1;
        private const int BACKWARD = -1;
        private const float LOOKAHEAD_DISTANCE_MAX = 1f;
        private const float UPDATE_INTERVAL = 0.05f;

        private GrindableRail _currentRail;
        private GrindableRail.RailPoint _targetPoint;
        private GrindableRail.RailPoint _closestPoint;
        private int _grindDirection;

        public GrindState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.GRIND) { }

        public override void Enter()
        {
            base.Enter();

            // Find the detected grindable rail
            controller.GrindableSensor.GetClosestCollider(out Collider closestCollider);
            if (closestCollider == null || closestCollider.GetComponent<GrindableRail>() == null)
            {
                Debug.LogError("Could not find rail");
                return;
            }

            // Set the current rail and target point
            _currentRail = closestCollider.GetComponent<GrindableRail>();
            _targetPoint = _currentRail.GetClosestRailPointToPosition(controller.CurrentPosition);

            // Determine the enter direction
            _currentRail.CalculateEnterDirection(
                _targetPoint,
                controller.CurrentDirection.Combined,
                out _grindDirection
            );

            // Start the staggered update coroutine
            controller.StartCoroutine(StaggeredUpdate());

            Debug.Log(
                $"Entered GrindState for rail: {_currentRail.name}, target point: {_targetPoint.Progress}, direction: {(_grindDirection == FORWARD ? "Forward" : "Backward")}"
            );
        }

        public override void Execute()
        {
            base.Execute();
            if (_currentRail == null)
            {
                Debug.LogError("No rail found");
                return;
            }

            if (_targetPoint.IsNull)
            {
                Debug.LogError("No target point found");
                return;
            }

            UpdateGrindMovement();
        }

        public override void DrawGizmos()
        {
            // -- ( Draw Target Point ) ---------------------------------------------------------------------------------
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_targetPoint.WorldPosition, 0.2f);
            Gizmos.DrawLine(controller.CurrentPosition, _targetPoint.WorldPosition);

            // -- ( Draw Closest Point ) ---------------------------------------------------------------------------------
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(_closestPoint.WorldPosition, 0.2f);
            Gizmos.DrawLine(controller.CurrentPosition, _closestPoint.WorldPosition);
        }

        private void UpdateGrindMovement()
        {
            _closestPoint = _currentRail.GetClosestRailPointToPosition(controller.CurrentPosition);

            float currentSpeed = controller.CurrentVelocity.Magnitude;
            float targetSpeed = controller.Settings.GrindSpeed;

            // Get input-based speed modification
            float inputSpeedModifier = GetInputSpeedModifier();
            targetSpeed *= inputSpeedModifier;

            controller.ExecuteFullVelocityToPosition(_targetPoint.WorldPosition, targetSpeed);

            UpdatePlayerHeight();

            float controllerDistanceToTarget = Vector3.Distance(
                controller.CurrentPosition,
                _targetPoint.WorldPosition
            );

            if (controllerDistanceToTarget < LOOKAHEAD_DISTANCE_MAX)
            {
                GrindableRail.RailPoint nextPoint = _targetPoint;
                if (_grindDirection == FORWARD)
                {
                    _currentRail.GetNextRailPoint(_targetPoint, out nextPoint, 1);
                }
                else
                {
                    _currentRail.GetPreviousRailPoint(_targetPoint, out nextPoint, 1);
                }

                if (!nextPoint.IsNull)
                {
                    _targetPoint = nextPoint;
                }

                if ((_targetPoint.IsEnd && _grindDirection == FORWARD) ||
                    (_targetPoint.IsStart && _grindDirection == BACKWARD))
                {
                    stateMachine.GoToState(MovementState.GRIND_JUMP);
                }
            }
        }

        /// <summary>
        /// Calculates a speed modifier based on player input.
        /// Forward input (positive Y) increases speed.
        /// Backward input (negative Y) decreases speed.
        /// </summary>
        private float GetInputSpeedModifier()
        {
            const float MIN_SPEED_MULTIPLIER = 0.5f;
            const float MAX_SPEED_MULTIPLIER = 1.5f;

            // Get the raw input value (-1 to 1)
            float inputY = controller.MoveInput.y;

            // Map the input to our desired speed range
            // No input (0) = 1.0 (normal speed)
            // Forward input (1) = 1.5 (50% faster)
            // Backward input (-1) = 0.5 (50% slower)
            return Mathf.Lerp(MIN_SPEED_MULTIPLIER, MAX_SPEED_MULTIPLIER, (inputY + 1f) * 0.5f);
        }

        private void UpdatePlayerHeight()
        {
            // -- ( Get Closest Spline Position ) ---------------------------------------------------------------------
            Vector3 currentPosition = controller.transform.position;
            _currentRail.CalculateClosestSplinePosition(
                currentPosition,
                out Vector3 closestSplinePos,
                out float splineProgress
            );

            // -- ( Update Player Height ) -----------------------------------------------------------------------------
            float targetY = closestSplinePos.y;
            float snapSpeed = controller.Settings.RailSnapSpeed;
            controller.transform.position = new Vector3(
                currentPosition.x,
                Mathf.Lerp(currentPosition.y, targetY, snapSpeed * Time.deltaTime),
                currentPosition.z
            );
        }

        IEnumerator StaggeredUpdate()
        {
            bool isRunning = true;
            float timer = 0f;
            while (isRunning)
            {
                yield return new WaitForSeconds(UPDATE_INTERVAL);
                timer += UPDATE_INTERVAL;
            }
        }

        private void ThrowOffRail()
        {
            stateMachine.GoToState(MovementState.GRIND_JUMP);
        }
    }
}
