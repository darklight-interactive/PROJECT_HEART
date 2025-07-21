using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ProjectHeart
{
    public class BossStateMachine : MonoBehaviour
    {
        private enum BOSS_PHASES { START, PHASE_01, PHASE_02, PHASE_03, DEAD }

        [Header("Refrences")]
        [SerializeField] private GameObject Player;

        [Header("Stats")]
        [SerializeField] private BossStats bossStats;



        [Header("RigidBody")]
        [SerializeField] private Rigidbody rb;

        [Header("Animator")] private Animator animator;

        [Header("UI/UX")]

        [SerializeField] private Slider bossSlider;

        [Header("Phases")]
        [SerializeField] private float attackDuration;
        [SerializeField] private BOSS_PHASES currentPhase;

        public UnityEvent OnHurt;
        public UnityEvent PHASE_01;
        public UnityEvent PHASE_02;
        public UnityEvent PHASE_03;
        public UnityEvent OnDeath;


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


        bool isKeyPressed;


        void Awake()
        {
            currentPhase = BOSS_PHASES.START;
            health = bossStats.Health;

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
        }

        void FixedUpdate()
        {

        }



        #region State Machine

        public void UpdateState()
        {
            switch (currentPhase)
            {
                case BOSS_PHASES.PHASE_01:
                    PHASE_01?.Invoke();
                    if (Mathf.Approximately(health, 66f))
                        currentPhase = BOSS_PHASES.PHASE_02;
                    break;
                case BOSS_PHASES.PHASE_02:
                    PHASE_02?.Invoke();
                    if (Mathf.Approximately(health, 33f))
                        currentPhase = BOSS_PHASES.PHASE_02;

                    break;
                case BOSS_PHASES.PHASE_03:
                    PHASE_03?.Invoke();
                    if (Mathf.Approximately(health, 10f))
                        currentPhase = BOSS_PHASES.PHASE_02;
                    break;
                case BOSS_PHASES.DEAD:
                    OnDeath?.Invoke();
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
    }
}
