namespace RemixSurvivors.Survivor
{
    public class IdleState : BaseState
    {
        public IdleState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.IDLE) { }

        public override void Enter()
        {
            base.Enter();
            controller.OnPrimaryInteractStarted += HandleJumpInput;
        }

        public override void Execute()
        {
            base.Execute();
            if (controller.IsMoveInputActive)
            {
                stateMachine.GoToState(MovementState.GROUND_SKATE);
            }
        }

        public override void Exit()
        {
            base.Exit();
            controller.OnPrimaryInteractStarted -= HandleJumpInput;
        }
    }
} 