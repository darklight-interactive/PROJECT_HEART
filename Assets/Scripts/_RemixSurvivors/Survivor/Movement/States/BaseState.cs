using Darklight.UnityExt.Behaviour;

namespace RemixSurvivors.Survivor
{
    public abstract class BaseState : FiniteState<MovementState>
    {
        protected readonly SurvivorMovementStateMachine stateMachine;
        protected readonly SurvivorMovementController controller;
        protected readonly SurvivorMovementSettings settings;

        public BaseState(FiniteStateMachine<MovementState> machine, MovementState state)
            : base(state)
        {
            stateMachine = machine as SurvivorMovementStateMachine;
            controller = stateMachine.Controller;
            settings = controller.Settings;
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
