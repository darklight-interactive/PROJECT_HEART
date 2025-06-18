using UnityEngine;

namespace RemixSurvivors.AI
{
    public abstract class Consideration : ScriptableObject
    {
        public abstract float Evaluate(Context context);
    }
}
