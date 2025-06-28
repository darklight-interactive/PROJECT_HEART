namespace RemixSurvivors.Survivor
{
    public class GrindJumpState : BaseState
    {
        float _elapsedJumpTime;

        public GrindJumpState(SurvivorMovementStateMachine machine)
            : base(machine, MovementState.GRIND_JUMP) { }

                    public override void Enter()
        {
            base.Enter();
            _elapsedJumpTime = 0f;
            controller.GroundSensor.Disable(settings.JumpBoostDuration);
        }

        public override void Execute()
        {
            base.Execute();

            controller.ExecuteHorizontalMovement_InAirDamping();
            controller.ExecuteVerticalMovement_JumpInitialBoost(
                ref _elapsedJumpTime,
                out bool isJumpTimeComplete
            );

            if (isJumpTimeComplete)
            {
                if (controller.GroundSensor.IsColliding)
                {
                    stateMachine.GoToState(MovementState.GROUND_SKATE);
                }
                else
                {
                    stateMachine.GoToState(MovementState.FALLING);
                }
            }
        }
    }
} 