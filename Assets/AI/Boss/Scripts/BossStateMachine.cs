using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ProjectHeart
{
    public class BossStateMachine : MonoBehaviour
    {
        public enum BOSS_PHASES { START, PHASE_01, PHASE_02, PHASE_03, DEAD }

        [Header("Refrences")]

        [SerializeField] private GameObject player;

        public GameObject Player => player;

        [Header("Stats")]
        [SerializeField] private BossStats bossStats;

        public BossStats BossStats => bossStats;


        [Header("RigidBody")]
        [SerializeField] private Rigidbody rb;

        [Header("Animator")]
        [SerializeField] private Animator animator;

        public Animator Animator => animator;


        [Header("RayCast")]

        [SerializeField] private Ray playerRay;
        [SerializeField] private LayerMask playerLayer;

        [SerializeField] private Vector3 direction;

        [Header("UI/UX")]

        [SerializeField] private Slider bossSlider;

        [Header("Phases")]
        [SerializeField] private float attackDuration;
        [SerializeField] private BOSS_PHASES currentPhase;


        // public UnityEvent OnHurt;
        // public UnityEvent PHASE_01;
        // public UnityEvent PHASE_02;
        // public UnityEvent PHASE_03;
        // public UnityEvent OnDeath;

        [SerializeField] private AttackPhaseState phase1State;




        private float health;
        private float speed;
        private float strength;

        public float Health
        {
            get { return health; }
            set
            {
                health = value;
                if (bossSlider.maxValue < health)
                {
                    bossSlider.maxValue = health;
                }
                bossSlider.value = health;

            }
        }


        public bool isBeingAttacked = true;
        public AudioSource hurtSfx;

        void Awake()
        {
            currentPhase = BOSS_PHASES.PHASE_01;
            Health = bossStats.Health;
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Check Health
            // Update to appropriate state

            UpdateState();


            // if (isBeingAttacked)
            // {
            //     Health -= .5f * Time.deltaTime;
            //     HandleOnHurt();

            // }
            // Debug.Log($"Health {Health}");

        }

        void FixedUpdate()
        {
            // MoveTowardsPlayer();
            // RotateTowardsPlayer();

        }

        private void MoveTowardsPlayer()
        {

            Vector3 targetPosition = player.gameObject.transform.position;

            float singleStep = bossStats.Speed * Time.fixedDeltaTime;
            if (Vector3.Distance(transform.position, targetPosition) > 3.5f)
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, singleStep);



        }

        private void RotateTowardsPlayer()
        {
            // Rotate toward player move to player
            Vector3 directionToPlayer = player.transform.position - transform.position;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                float singleStep = 100f * Time.fixedDeltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
            }

        }



        #region State Machine

        public void UpdateState()
        {
            switch (currentPhase)
            {
                case BOSS_PHASES.PHASE_01:
                    if (phase1State == null)
                    {
                        phase1State = new AttackPhaseState(this);
                    }

                    phase1State.Tick();
                    if (Mathf.Approximately(health, 66f))
                        currentPhase = BOSS_PHASES.PHASE_02;

                    break;
                case BOSS_PHASES.PHASE_02:
                    // PHASE_02?.Invoke();
                    if (Mathf.Approximately(health, 33f))
                        currentPhase = BOSS_PHASES.PHASE_02;

                    break;
                case BOSS_PHASES.PHASE_03:
                    // PHASE_03?.Invoke();
                    if (Mathf.Approximately(health, 10f))
                        currentPhase = BOSS_PHASES.PHASE_02;
                    break;
                case BOSS_PHASES.DEAD:
                    // OnDeath?.Invoke();
                    break;
            }
        }


        public void OnGameStateChanged()
        {

        }
        #endregion

        // #region Animation Triggers

        // #endregion

        // #region Movement
        // #endregion

        public void HandleOnHurt()
        {

            // animator.SetTrigger("Hurt");
            return;
        }

        public void PlayHurtSFX()
        {
            hurtSfx.Stop();
            hurtSfx.Play();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, direction);

        }
    }
}
