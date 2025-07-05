namespace ProjectHeart.Character
{
    public partial class CharacterMovementController
    {
        public class GroundMoveState : BaseState
        {
            public GroundMoveState(MovementStateMachine machine)
                : base(machine, MovementState.GROUND_MOVE) { }

            public override void Enter()
            {
                base.Enter();
                input.Jump += HandleJumpInput;
            }

            public override void Execute()
            {
                base.Execute();

                // Move the character horizontally based on the input.
                controller.ExecuteHorizontalMovement_GroundMoveAcceleration();

                // Apply constant gravity to the character.
                controller.ExecuteVerticalMovement_GravityConstant();

                if (!input.IsMoveActive)
                {
                    stateMachine.GoToState(MovementState.IDLE);
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
