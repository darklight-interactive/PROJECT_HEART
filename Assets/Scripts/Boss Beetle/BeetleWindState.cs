using UnityEngine;

namespace ProjectHeart
{
    // public partial class [StateManagerName] {}
    
    public class BeetleWindState : BeetleBaseState
    {
        public BeetleWindState(BeetleStateManager beetle) : base(beetle) { }

        // In this state, the bug is simply waiting with a passive animation
        // Once the player enters either its range or the arena, it will change to "winding state"

        public override void Enter()
        {
            //base.Enter();

            // Loop passive animation
            beetle.animator.SetTrigger("BeetleWind");

            // Start countdown function to winding time ending, either hard-coded time or based on animation loops
            // In the coroutine, once time is up enter the attack state
        }

        public override void Execute()
        {
            //base.Execute();
        }

        public override void Exit()
        {
            //base.Exit();

            //beetle.animator.Stop("BeetleWinding");

            // Stop passive animation? or new state will start new animation and override it?
        }
    }
}
