using Darklight.UnityExt.Behaviour;
using ProjectHeart.Input;

namespace ProjectHeart.Character
{
    public partial class CharacterMovement
    {
        public abstract class BaseState : FiniteState<MovementState>
        {
            protected readonly StateMachine stateMachine;
            protected readonly CharacterMovement controller;
            protected readonly CharacterMovementSettings settings;
            protected readonly InputActionMap_Player input;

            public BaseState(StateMachine machine, MovementState state)
                : base(state)
            {
                stateMachine = machine as StateMachine;
                controller = stateMachine.Controller;
                settings = controller.Settings;
                input = GlobalInputReader.PlayerInput;
            }

            public override void Enter()
            {
                base.Enter();

                controller.OnDrawGizmosEvent += DrawGizmos;
                controller.OnDrawGizmosSelectedEvent += DrawGizmosSelected;
            }

            public override void Execute()
            {
                base.Execute();
            }

            public override void Exit()
            {
                base.Exit();

                controller.OnDrawGizmosEvent -= DrawGizmos;
                controller.OnDrawGizmosSelectedEvent -= DrawGizmosSelected;
            }

            protected virtual void HandleJumpInput()
            {
                if (controller.GroundSensor.IsColliding)
                {
                    stateMachine.GoToState(MovementState.GROUND_JUMP);
                }
            }

            public virtual void DrawGizmos() { }

            public virtual void DrawGizmosSelected() { }
        }
    }
}
