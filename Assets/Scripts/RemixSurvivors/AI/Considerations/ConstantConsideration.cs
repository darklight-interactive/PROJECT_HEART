using UnityEngine;

namespace RemixSurvivors.AI
{
    [CreateAssetMenu(menuName = "RemixSurvivors.AI/Considerations/Constant")]
    public class ConstantConsideration : Consideration
    {
        public float value;

        public override float Evaluate(Context context) => value;
    }
}
