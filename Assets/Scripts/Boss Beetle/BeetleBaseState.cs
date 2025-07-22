using UnityEngine;

namespace ProjectHeart
{
    public abstract class BeetleBaseState
    {
        protected BeetleStateManager beetle;

        public BeetleBaseState(BeetleStateManager beetle)
        {
            this.beetle = beetle;
        }

        public virtual void Enter() { }
        public virtual void Execute() { }
        public virtual void Exit() { }
    }
}
