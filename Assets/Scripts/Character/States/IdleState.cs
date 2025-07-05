namespace ProjectHeart.Character
{
    public partial class CharacterMovementController
    {
        public class IdleState : BaseState
        {
            public IdleState(MovementStateMachine machine)
                : base(machine, MovementState.IDLE) { }

            public override void Enter()
            {
                base.Enter();
                input.Jump += HandleJumpInput;
            }

            public override void Execute()
            {
                base.Execute();

                // Decelerate the character horizontally to zero.
                controller.ExecuteHorizontalMovement_GroundMoveDeceleration();

                // Apply constant gravity to the character.
                controller.ExecuteVerticalMovement_GravityConstant();

                if (input.IsMoveActive)
                {
                    stateMachine.GoToState(MovementState.GROUND_MOVE);
                }
            }

            public override void Exit()
            {
                base.Exit();
                input.Jump -= HandleJumpInput;
            }
        }
    }
}
