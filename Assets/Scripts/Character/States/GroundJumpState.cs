namespace ProjectHeart.Character
{
    public partial class CharacterMovement
    {
        public class GroundJumpState : BaseState
        {
            public float elapsedJumpTime;

            public GroundJumpState(StateMachine machine)
                : base(machine, MovementState.GROUND_JUMP) { }

            public override void Enter()
            {
                base.Enter();
                elapsedJumpTime = 0f;
                controller.GroundSensor.Disable(settings.JumpBoostDuration);
            }

            public override void Execute()
            {
                base.Execute();

                controller.ExecuteHorizontalMovement_InAirDamping();
                controller.ExecuteVerticalMovement_JumpInitialBoost(
                    ref elapsedJumpTime,
                    out bool isJumpTimeComplete
                );

                if (isJumpTimeComplete)
                {
                    if (controller.GroundSensor.IsColliding)
                    {
                        stateMachine.GoToState(MovementState.GROUND_MOVE);
                    }
                    else
                    {
                        stateMachine.GoToState(MovementState.FALLING);
                    }
                }
            }
        }
    }
}
