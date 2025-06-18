using UnityEngine;

namespace RemixSurvivors.AI
{
    /// <summary>
    /// Moves the agent to the closest target with the specified tag.
    /// </summary>
    [CreateAssetMenu(menuName = "RemixSurvivors.AI/Actions/MoveToTargetAction")]
    public class MoveToTargetAIAction : AIAction
    {
        public override void Initialize(Context context)
        {
            context.Sensor.TryAddTag(TargetTag);
        }

        public override void Execute(Context context)
        {
            var target = context.Sensor.GetClosestTarget(TargetTag);
            if (target == null)
                return;

            context.Agent.SetDestination(target.position);
        }
    }
}
