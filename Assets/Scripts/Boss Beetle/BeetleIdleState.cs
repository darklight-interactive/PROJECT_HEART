using UnityEngine;

namespace ProjectHeart
{
    // public partial class [StateManagerName] {}
    
    public class BeetleIdleState : BeetleBaseState
    {
        public BeetleIdleState(BeetleStateManager beetle) : base(beetle) { }

        // In this state, the bug is simply waiting with a passive animation
        // Once the player enters either its range or the arena, it will change to "winding state"

        public override void Enter()
        {
            //base.Enter();

            // Loop passive animation
            beetle.animator.Play("BeetleIdle");
        }

        public override void Execute()
        {
            //base.Execute();

            // When player enters the arena, enter "winding state"
        }

        public override void Exit()
        {
            //base.Exit();

            //beetle.animator.Stop("BeetleIdle");

            // Stop passive animation? or new state will start new animation and override it?
        }
    }
}
