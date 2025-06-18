using System.Collections;
using RemixSurvivors.HealthSystem;
using UnityEngine;

namespace RemixSurvivors
{
    public class EntityHealthController : MonoBehaviour
    {
        Entity _entity;

        [SerializeField]
        Health _health;

        public float CurrentHealth => _health.CurrentHealth;

        public void Awake()
        {
            _entity = GetComponent<Entity>();

            _health.SetHealth(_health.MaxHealth);
            //_healthBar.Initialize(_health.MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            Debug.Log(
                $"Entity Took damage: {amount}, current health: {_health.CurrentHealth}",
                gameObject
            );
            bool result = _entity.StateMachine.TryGoToState(EntityState.DAMAGE);
            if (result)
            {
                _entity.StateMachine.PlayDamageParticles();
                _health.CurrentHealth -= amount;
                //  _healthBar.UpdateBar(_health.CurrentHealth);
            }

            if (_health.CurrentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            StartCoroutine(DestroyAfterSeconds(2f));
        }

        IEnumerator DestroyAfterSeconds(float seconds)
        {
            _entity.StateMachine.ForceGoToState(EntityState.DEAD);
            _entity.StateMachine.PlayDeadParticles();

            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
            _entity.StateMachine.PlayDeadParticles();
        }
    }
}
