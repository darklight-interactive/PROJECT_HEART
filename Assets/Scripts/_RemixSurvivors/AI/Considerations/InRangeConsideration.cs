using UnityEngine;
using UnityUtils;

namespace RemixSurvivors.AI
{
    [CreateAssetMenu(menuName = "RemixSurvivors.AI/Considerations/InRangeConsideration")]
    public class InRangeConsideration : Consideration
    {
        public float maxDistance = 10f;

        [Range(15, 360)]
        public float maxAngle = 360f;
        public string targetTag = "Target";
        public AnimationCurve curve;

        public override float Evaluate(Context context)
        {
            if (context == null)
            {
                Debug.LogError("Context is null", this);
                return 0f;
            }

            if (context.Sensor == null)
            {
                Debug.LogError("Sensor is null", this);
                return 0f;
            }

            context.Sensor.TryAddTag(targetTag);

            Transform targetTransform = context.Sensor.GetClosestTarget(targetTag);
            if (targetTransform == null)
                return 0f;

            Transform agentTransform = context.Agent.transform;

            bool isInRange = agentTransform.InRangeOf(targetTransform, maxDistance, maxAngle);
            if (!isInRange)
                return 0f;

            Vector3 directionToTarget = targetTransform.position - agentTransform.position;
            float distanceToTarget = directionToTarget.With(y: 0).magnitude;

            float normalizedDistance = Mathf.Clamp01(distanceToTarget / maxDistance);

            float utility = curve.Evaluate(normalizedDistance);
            return Mathf.Clamp01(utility);
        }

        void Reset()
        {
            curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
        }
    }
}
