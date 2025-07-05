using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace RemixSurvivors.AI
{
    public abstract class AIAction : ScriptableObject
    {
        [SerializeField, GroupedTag]
        string _targetTag;

        [SerializeField, Expandable]
        Consideration _consideration;

        public string TargetTag => _targetTag;
        public Consideration Consideration => _consideration;

        /// <summary>
        /// Optional initialization method that can be used to set up the action.
        /// Called once when the brain's Awake method is called.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Initialize(Context context) { }

        /// <summary>
        /// Calculates the utility of the action based on the given context.
        /// </summary>
        /// <param name="context">The context of the action.</param>
        ///     <returns>The utility of the action.</returns>
        public float CalculateUtility(Context context) => _consideration.Evaluate(context);

        /// <summary>
        /// Executes the action based on the given context.
        /// </summary>
        /// <param name="context">The context of the action.</param>
        public abstract void Execute(Context context);
    }
}
