using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace RemixSurvivors
{
    /// <summary>
    /// Manages spawning of objects in random positions within a circular area,
    /// ensuring spawn positions are clear of obstacles.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float checkSphereRadius = 0.5f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private int spawnCount = 10;
        
        [Header("Timing Settings")]
        [SerializeField] private float timeBetweenSpawnWaves = 5f;
        [SerializeField] private bool autoSpawn = true;
        
        [Header("Prefab Settings")]
        [SerializeField, Required] private GameObject prefabToSpawn;

        private Coroutine spawnCoroutine;
        
        private void Start()
        {
            if (autoSpawn)
            {
                StartSpawning();
            }
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        /// <summary>
        /// Starts the continuous spawning process.
        /// </summary>
        [Button]
        public void StartSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopSpawning();
            }
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        /// <summary>
        /// Stops the continuous spawning process.
        /// </summary>
        [Button]
        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        /// <summary>
        /// Coroutine that handles continuous spawning of prefabs.
        /// </summary>
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnPrefabsInSafePositions();
                yield return new WaitForSeconds(timeBetweenSpawnWaves);
            }
        }

        /// <summary>
        /// Attempts to spawn prefabs in random positions within the defined circular area.
        /// Only spawns at positions where no obstacles are detected.
        /// </summary>
        [Button]
        private void SpawnPrefabsInSafePositions()
        {
            if (prefabToSpawn == null)
            {
                Debug.LogError("No prefab assigned to spawn!");
                return;
            }

            List<Vector3> safePositions = FindSafeSpawnPositions();
            
            foreach (Vector3 position in safePositions)
            {
                SpawnPrefab(position);
            }
        }

        /// <summary>
        /// Finds safe spawn positions within the specified radius.
        /// </summary>
        /// <returns>List of safe spawn positions. May contain fewer positions than requested if safe spots cannot be found.</returns>
        private List<Vector3> FindSafeSpawnPositions()
        {
            List<Vector3> safePositions = new List<Vector3>();
            int attempts = 0;
            const int maxAttempts = 30; // Prevent infinite loops

            while (safePositions.Count < spawnCount && attempts < maxAttempts)
            {
                Vector3 randomPosition = GetRandomPositionInCircle();
                
                if (IsSafePosition(randomPosition))
                {
                    safePositions.Add(randomPosition);
                }
                
                attempts++;
            }

            return safePositions;
        }

        /// <summary>
        /// Generates a random position within the spawn circle on the XZ plane.
        /// </summary>
        /// <returns>A random position within the spawn circle.</returns>
        private Vector3 GetRandomPositionInCircle()
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return new Vector3(randomCircle.x, 0f, randomCircle.y) + transform.position;
        }

        /// <summary>
        /// Checks if a position is safe to spawn (no colliders within the check sphere).
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is safe to spawn, false otherwise.</returns>
        private bool IsSafePosition(Vector3 position)
        {
            return !Physics.CheckSphere(position, checkSphereRadius, obstacleLayer);
        }

        /// <summary>
        /// Spawns the assigned prefab at the specified position.
        /// </summary>
        /// <param name="position">The position to spawn the prefab.</param>
        /// <returns>The spawned GameObject instance.</returns>
        private GameObject SpawnPrefab(Vector3 position)
        {
            return Instantiate(prefabToSpawn, position, Quaternion.identity);
        }

        /// <summary>
        /// Draws the spawn radius gizmo in the editor for visualization.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}