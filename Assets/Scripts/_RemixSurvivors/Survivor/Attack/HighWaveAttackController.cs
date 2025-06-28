using NaughtyAttributes;
using UnityEngine;

namespace RemixSurvivors
{
    /// <summary>
    /// Controls a high-wave attack pattern that spawns projectiles in eight directions on the XZ plane
    /// </summary>
    public class HighWaveAttackController : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private Vector3 projectileScale = Vector3.one;
        
        [Header("Spawn Settings")]
        [SerializeField] private Vector3 spawnOffset = Vector3.up;
        
        private int currentDirectionIndex = 0;
        
        private readonly Vector3[] directions = new Vector3[]
        {
            Vector3.forward,                          // N
            (Vector3.forward + Vector3.right).normalized,  // NE
            Vector3.right,                            // E
            (Vector3.right + Vector3.back).normalized,     // SE
            Vector3.back,                             // S
            (Vector3.back + Vector3.left).normalized,      // SW
            Vector3.left,                             // W
            (Vector3.left + Vector3.forward).normalized    // NW
        };

        /// <summary>
        /// Spawns a single cube projectile in the next clockwise direction
        /// </summary>
        [Button]
        public void FireProjectile()
        {
            Vector3 direction = directions[currentDirectionIndex];
            Vector3 spawnPosition = transform.position + spawnOffset + direction * 2f;

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            projectile.transform.position = spawnPosition;
            projectile.transform.localScale = projectileScale;
            
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = direction * projectileSpeed;

            Destroy(projectile, 5f);

            // Move to next direction (clockwise)
            currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
        }

        /// <summary>
        /// Spawns cube projectiles in all eight directions from the current position
        /// </summary>
        [Button]
        public void FireAllProjectiles()
        {
            for (int i = 0; i < directions.Length; i++)
            {
                FireProjectile();
            }
        }
    }
}
