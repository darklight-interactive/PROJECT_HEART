namespace ProjectHeart.Character
{
    public partial class CharacterMovement
    {
        public class FallingState : BaseState
        {
            public FallingState(StateMachine machine)
                : base(machine, MovementState.FALLING) { }

            public override void Execute()
            {
                base.Execute();
                controller.ExecuteHorizontalMovement_InAirDamping();
                controller.ExecuteVerticalMovement_GravityConstant();

                if (controller.GroundSensor.IsColliding)
                {
                    stateMachine.GoToState(MovementState.GROUND_MOVE);
                }
            }
        }
    }
}
