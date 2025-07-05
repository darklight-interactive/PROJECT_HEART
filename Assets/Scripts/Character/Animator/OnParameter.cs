using UnityEngine;

namespace ProjectHeart.Character
{
    public class OnParameter : StateMachineBehaviour
    {
        [SerializeField, Tooltip("Parameter to test")]
        private Parameters parameter;

        [SerializeField, Tooltip("Specify whether it should be on or off")]
        private bool target;

        [SerializeField, Tooltip("Chain of animations to play when condition is met")]
        private AnimationData[] nextAnimations;

        private ManualAnimator animatorBrain;

        public override void OnStateEnter(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            animatorBrain = animator.GetComponent<ManualAnimator>();
        }

        public override void OnStateUpdate(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            if (animatorBrain.GetBool(parameter) != target)
                return;
            animatorBrain.SetLocked(false, layerIndex);

            for (int i = 0; i < nextAnimations.Length - 1; ++i)
                nextAnimations[i].nextAnimation = nextAnimations[i + 1];

            animatorBrain.Play(nextAnimations[0], layerIndex);
        }
    }
}
