using RemixSurvivors.HealthSystem;
using UnityEngine;

namespace RemixSurvivors
{
    public class SurvivorHealthController : MonoBehaviour
    {
        [SerializeField]
        Health _health;

        public void Awake()
        {
            _health.SetHealth(_health.MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            _health.CurrentHealth -= amount;

            Debug.Log($"Player Took damage: {amount}, current health: {_health.CurrentHealth}");

            if (_health.CurrentHealth <= 0)
            {
                Die();
            }
        }

        void Die() { }
    }
}
