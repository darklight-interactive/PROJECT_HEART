using UnityEditor;
using UnityEngine;

namespace RemixSurvivors.AI
{
    [CustomEditor(typeof(Brain), true)]
    public class BrainCustomEditor : UnityEditor.Editor
    {
        public virtual void OnEnable()
        {
            this.RequiresConstantRepaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Draw the default inspector

            Brain brain = (Brain)target;

            if (Application.isPlaying)
            {
                AIAction chosenAction = brain.GetBestAction();
                if (chosenAction != null)
                {
                    EditorGUILayout.LabelField($"Best Action: {chosenAction.GetType().Name}", EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("No best action found", EditorStyles.boldLabel);
                }

                if (chosenAction != null)
                {
                    EditorGUILayout.LabelField(
                        $"Current Chosen Action: {chosenAction.GetType().Name}",
                        EditorStyles.boldLabel
                    );
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions/Considerations", EditorStyles.boldLabel);

                foreach (AIAction action in brain.Actions)
                {
                    float utility = action.CalculateUtility(brain.Context);
                    EditorGUILayout.LabelField($"Action: {action.name}, Utility: {utility:F2}");

                    // Draw the single consideration for the action
                    DrawConsideration(action.Consideration, brain.Context, 1);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Enter Play mode to view utility values.",
                    MessageType.Info
                );
            }
        }

        private void DrawConsideration(
            Consideration consideration,
            Context context,
            int indentLevel
        )
        {
            EditorGUI.indentLevel = indentLevel;

            if (consideration is CompositeConsideration compositeConsideration)
            {
                EditorGUILayout.LabelField(
                    $"Composite Consideration: {compositeConsideration.name}, Operation: {compositeConsideration.operation}"
                );

                foreach (Consideration subConsideration in compositeConsideration.considerations)
                {
                    DrawConsideration(subConsideration, context, indentLevel + 1);
                }
            }
            else
            {
                float value = consideration.Evaluate(context);
                EditorGUILayout.LabelField(
                    $"Consideration: {consideration.name}, Value: {value:F2}"
                );
            }

            EditorGUI.indentLevel = indentLevel - 1; // Reset indentation after drawing
        }

    }
}
