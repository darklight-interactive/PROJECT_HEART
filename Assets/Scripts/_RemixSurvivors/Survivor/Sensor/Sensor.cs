using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RemixSurvivors.Survivor
{
    [System.Serializable]
    public class Sensor
    {
        protected SensorSettings settings;
        protected SurvivorMovementController survivor;
        protected Transform transform;
        protected Vector3 position;

        [SerializeField, ReadOnly, AllowNesting]
        protected LayerMask layerMask;

        [SerializeField, ReadOnly, AllowNesting]
        protected float overlapRadius;

        [SerializeField, ReadOnly, AllowNesting]
        protected bool isDisabled;

        [SerializeField, ShowOnly]
        protected float lastUpdateTime;

        [SerializeField, ReadOnly, AllowNesting]
        protected List<Collider> colliders = new List<Collider>();

        public bool IsDisabled => isDisabled;
        public bool IsColliding => CheckCollision();
        public List<Collider> Colliders => colliders;

        public Sensor(SensorSettings settings, SurvivorMovementController survivor)
        {
            if (settings == null)
            {
                Debug.LogError("Sensor settings are null");
                isDisabled = true;
                return;
            }

            if (survivor == null)
            {
                Debug.LogError("Survivor is null");
                isDisabled = true;
                return;
            }

            this.settings = settings;
            this.survivor = survivor;
            transform = survivor.transform;
            position = transform.position;
            overlapRadius = settings.Radius;
            layerMask = settings.Layer;
        }

        public virtual void Execute()
        {
            if (isDisabled)
                return;

            lastUpdateTime = Time.time;
            position = transform.position;
            colliders = GetCurrentColliders();
        }

        public virtual void Disable(float duration)
        {
            if (isDisabled)
                return;
            survivor.StartCoroutine(DisableRoutine(duration));
        }

        public void GetClosestCollider(out Collider closestCollider)
        {
            closestCollider = colliders
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .FirstOrDefault();
        }

        protected bool CheckCollision()
        {
            if (isDisabled)
                return false;
            return Physics.CheckSphere(position, overlapRadius, layerMask);
        }

        protected virtual List<Collider> GetCurrentColliders()
        {
            return new List<Collider>(Physics.OverlapSphere(position, overlapRadius, layerMask));
        }

        protected IEnumerator DisableRoutine(float duration)
        {
            isDisabled = true;
            yield return new WaitForSeconds(duration);
            isDisabled = false;
        }

#if UNITY_EDITOR
        public virtual void DrawGizmos()
        {
            if (settings == null || !settings.ShowDebugGizmos)
                return;

            Color color = Color.gray;
            if (IsColliding)
                color = settings.DebugCollidingColor;
            DrawOverlapRadius(color);
        }

        protected virtual void DrawOverlapRadius(Color gizmoColor)
        {
            Handles.color = gizmoColor;
            Handles.DrawWireDisc(position, Vector3.up, overlapRadius);
            Handles.DrawWireDisc(position, Vector3.forward, overlapRadius);
        }
#endif
    }
}
