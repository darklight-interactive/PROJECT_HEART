namespace RemixSurvivors.Survivor
{
    public class FallingState : BaseState
    {
        public FallingState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.FALLING) { }

        public override void Execute()
        {
            base.Execute();
            controller.ExecuteHorizontalMovement_InAirDamping();
            controller.ExecuteVerticalMovement_GravityConstant();

            if (controller.GroundSensor.IsColliding)
            {
                stateMachine.GoToState(MovementState.GROUND_SKATE);
            }
        }
    }
} 