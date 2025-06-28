namespace RemixSurvivors.Survivor
{
    public class GroundSkateBrakeState : BaseState
    {
        public GroundSkateBrakeState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.GROUND_SKATE_BRAKE) { }

        public override void Enter()
        {
            base.Enter();
            controller.OnPrimaryInteractStarted += HandleJumpInput;
        }

        public override void Execute()
        {
            base.Execute();
            controller.ExecuteHorizontalMovement_GroundSkateDeceleration();
            controller.ExecuteVerticalMovement_GravityConstant();

            if (controller.IsMoveInputActive)
            {
                stateMachine.GoToState(MovementState.GROUND_SKATE);
            }
            else if (controller.CurrentVelocity.Horizontal.magnitude <= 0.1f)
            {
                stateMachine.GoToState(MovementState.IDLE);
            }
        }

        public override void Exit()
        {
            base.Exit();
            controller.OnPrimaryInteractStarted -= HandleJumpInput;
        }
    }
} 