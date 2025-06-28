using System.Collections.Generic;
using Darklight.UnityExt.Behaviour;

namespace RemixSurvivors.Survivor
{
    public enum MovementState
    {
        /// <summary> The survivor is not moving. </summary>
        IDLE,

        /// <summary> The survivor is skating on the ground. </summary>
        GROUND_SKATE,

        /// <summary> The survivor is braking while skating on the ground. </summary>
        GROUND_SKATE_BRAKE,

        /// <summary> The survivor is jumping while on the ground. </summary>
        GROUND_JUMP,

        /// <summary> The survivor is grinding. </summary>
        GRIND,

        /// <summary> The survivor is jumping while grinding. </summary>
        GRIND_JUMP,

        /// <summary> The survivor is falling. </summary>
        FALLING
    }

    [System.Serializable]
    public class SurvivorMovementStateMachine : FiniteStateMachine<MovementState>
    {
        SurvivorMovementController _controller;
        public SurvivorMovementController Controller => _controller;

        public SurvivorMovementStateMachine(SurvivorMovementController controller)
            : base(MovementState.IDLE)
        {
            _controller = controller;
            /*
            OverrideFiniteStates(
                new Dictionary<MovementState, FiniteState<MovementState>>()
                {
                    { MovementState.IDLE, new IdleState(this) },
                    { MovementState.GROUND_SKATE, new GroundSkateState(this) },
                    { MovementState.GROUND_SKATE_BRAKE, new GroundSkateBrakeState(this) },
                    { MovementState.GROUND_JUMP, new GroundJumpState(this) },
                    { MovementState.GRIND, new GrindState(this) },
                    { MovementState.GRIND_JUMP, new GrindJumpState(this) },
                    { MovementState.FALLING, new FallingState(this) },
                }
            );
            */
        }
    }
}
