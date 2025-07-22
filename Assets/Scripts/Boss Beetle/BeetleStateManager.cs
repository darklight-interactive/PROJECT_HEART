using UnityEngine;

namespace ProjectHeart
{
    public enum BeetleState
    {
        /// <summary> The beetle is not moving. </summary>
        IDLE,

        /// <summary> The beetle is in a cooldown and preparing its next attack. </summary>
        WINDING,

        /// <summary> The beetle attacks by running towards the player. </summary>
        ATTACK_CHARGE

        /// <summary> The beetle's other attacks. Unsure how to implement at the moment. </summary>
        //ATTACK_TOXIN,

        //ATTACK_WINGS,

        //ATTACK_JUMP
    }

    public class BeetleStateManager : MonoBehaviour
    {
        private BeetleBaseState currentState;

        // Health
        private int currentHealth;

        public Collider triggerZone;

        public Animator animator;

        // Data object for later
        //BeetleAnimations beetleAnimations;

        public BeetleIdleState idleState;
        public BeetleWindState windState;
        //public BeetleState idleState;
        //public BeetleState windState;

        private void Awake()
        {
            idleState = new BeetleIdleState(this);
            windState = new BeetleWindState(this);
        }

        private void Start()
        {
            TransitionToState(idleState);
        }

        //private void Update()
        //{
        //    currentState?.Update();
        //}

        public void TransitionToState(BeetleBaseState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Detected trigger from: " + other);
            if (other.CompareTag("Player") && currentState == idleState)
            {
                TransitionToState(windState);
            }
        }
    }
}
