using UnityEngine;

namespace RemixSurvivors.AI
{
    [CreateAssetMenu(menuName = "RemixSurvivors.AI/Actions/IdleAction")]
    public class IdleAIAction : AIAction
    {
        public override void Execute(Context context)
        {
            context.Agent.SetDestination(context.Target.position);
        }
    }
}
