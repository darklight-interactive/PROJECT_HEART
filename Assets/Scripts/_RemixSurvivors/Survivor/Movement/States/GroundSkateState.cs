namespace RemixSurvivors.Survivor
{
    public class GroundSkateState : BaseState
    {
        public GroundSkateState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.GROUND_SKATE) { }

        public override void Enter()
        {
            base.Enter();
            controller.OnPrimaryInteractStarted += HandleJumpInput;
        }

        public override void Execute()
        {
            base.Execute();
            controller.ExecuteHorizontalMovement_GroundSkateAcceleration();
            controller.ExecuteVerticalMovement_GravityConstant();

            if (controller.IsMoveInputActive == false)
            {
                stateMachine.GoToState(MovementState.GROUND_SKATE_BRAKE);
            }

            if (controller.GrindableSensor.IsColliding)
            {
                stateMachine.GoToState(MovementState.GRIND);
            }
        }

        public override void Exit()
        {
            base.Exit();
            controller.OnPrimaryInteractStarted -= HandleJumpInput;
        }
    }
} 