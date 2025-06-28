using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace RemixSurvivors.AI
{
    [ExecuteAlways]
    public class Sensor : MonoBehaviour
    {
        private const float DEFAULT_REFRESH_RATE = 0.5f;

        // Cache components and frequently used values
        private readonly Collider[] _colliderBuffer = new Collider[50]; // Preallocate buffer for Physics.OverlapSphereNonAlloc
        private bool _isPreloaded;
        private float _lastScanTime;

        [SerializeField, Range(0f, 100f)]
        private float _detectionRange = 10f;

        [SerializeField, GroupedTag]
        private List<string> _targetTags = new();

        [HorizontalLine]
        [SerializeField, ReadOnly]
        private SerializedDictionary<string, List<Transform>> _detectedObjects = new();

        public Transform GetClosestTarget(string tag)
        {
            if (!_detectedObjects.TryGetValue(tag, out var targets) || targets.Count == 0)
                return null;

            Transform closestTarget = null;
            float closestDistanceSqr = float.MaxValue;
            Vector3 currentPosition = transform.position;

            if (targets.Count == 0)
                return null;

            foreach (Transform target in targets)
            {
                if (target == null)
                    continue;
                float dSqrToTarget = (target.position - currentPosition).sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        public void TryAddTag(string tag)
        {
            if (!_targetTags.Contains(tag))
                _targetTags.Add(tag);
        }

        public void TryAddTags(List<string> tags)
        {
            foreach (string tag in tags)
            {
                TryAddTag(tag);
            }
        }

        private void Awake()
        {
            if (!_isPreloaded)
                Preload();
        }

        [Button]
        private void Preload()
        {
            // Make sure there are no duplicates
            _targetTags = _targetTags.Distinct().ToList();

            _isPreloaded = true;
        }

        private void Update()
        {
            if (Time.time - _lastScanTime >= DEFAULT_REFRESH_RATE)
                Scan();
        }

        private void Scan()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _detectionRange,
                _colliderBuffer
            );

            for (int i = 0; i < hitCount; i++)
            {
                ProcessDetectedObject(_colliderBuffer[i]);
            }

            CleanupDetectedObjects();
            _lastScanTime = Time.time;
        }

        private void ProcessDetectedObject(Collider collider)
        {
            if (collider == null || collider.transform.IsChildOf(transform))
                return;

            if (_targetTags.Contains(collider.tag))
                TryAddDetectedObject(collider);
        }

        private void TryAddDetectedObject(Collider collider)
        {
            Transform targetTransform = collider.transform;
            if (targetTransform == null)
                return;

            // Add directly to correct tag list
            string tag = collider.tag;
            if (!_detectedObjects.TryGetValue(tag, out var transformList))
            {
                transformList = new List<Transform>();
                _detectedObjects[tag] = transformList;
            }

            if (!transformList.Contains(targetTransform))
                transformList.Add(targetTransform);
        }

        private void CleanupDetectedObjects()
        {
            float maxRangeSqr = _detectionRange * _detectionRange;
            Vector3 currentPos = transform.position;
            HashSet<Transform> processedObjects = new();

            foreach (var kvp in _detectedObjects.ToList())
            {
                var list = kvp.Value;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var target = list[i];
                    if (
                        target == null
                        || (target.position - currentPos).sqrMagnitude > maxRangeSqr
                        || !processedObjects.Add(target)
                    ) // Remove duplicates during cleanup
                    {
                        list.RemoveAt(i);
                    }
                }

                if (list.Count == 0)
                    _detectedObjects.Remove(kvp.Key);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }

#if UNITY_EDITOR
        [Button("Debug Detected Objects")]
        private void DebugDetectedObjects()
        {
            Debug.Log($"=== Detected Objects for {gameObject.name} ===");
            foreach (var kvp in _detectedObjects)
            {
                Debug.Log($"Tag: {kvp.Key}, Count: {kvp.Value.Count}");
                foreach (var obj in kvp.Value)
                {
                    Debug.Log($"- {obj.name} at position {obj.position}");
                }
            }

            ValidateNoDuplicates();
        }

        private bool ValidateNoDuplicates()
        {
            HashSet<Transform> allObjects = new();
            foreach (var kvp in _detectedObjects)
            {
                foreach (var transform in kvp.Value)
                {
                    if (!allObjects.Add(transform))
                    {
                        Debug.LogError(
                            $"Duplicate object found: {transform.name} in multiple tag lists!",
                            this
                        );
                        return false;
                    }
                }
            }
            return true;
        }
#endif
    }
}
