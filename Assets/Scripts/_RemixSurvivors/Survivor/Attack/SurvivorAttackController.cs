using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;

namespace RemixSurvivors
{
    public class SurvivorAttackController : MonoBehaviour
    {
        [SerializeField]
        LayerMask _enemyLayer;

        [SerializeField]
        float _attackRadius = 2f;

        [SerializeField]
        int _damageAmount = 10;

        [SerializeField, ReadOnly]
        Entity[] _enemiesInRange;

        public void AttackEnemiesInRadius()
        {
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                _attackRadius,
                _enemyLayer
            );
            _enemiesInRange = colliders.Select(c => c.GetComponent<Entity>()).ToArray();

            //Apply damage to all enemies in range
            foreach (Entity enemy in _enemiesInRange)
            {
                //if (!enemy.Context.Agent.isOnNavMesh) continue;
                if (enemy == null)
                {
                    Debug.LogError("Enemy is null");
                    continue;
                }
                if (enemy.HealthController == null)
                {
                    Debug.LogError("Enemy health controller is null");
                    continue;
                }

                enemy.HealthController.TakeDamage(_damageAmount);

                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                enemy.Knockback(direction);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRadius);
        }
    }
}
