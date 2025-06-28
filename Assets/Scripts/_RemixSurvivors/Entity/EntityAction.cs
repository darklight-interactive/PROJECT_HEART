using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using RemixSurvivors.AI;
using UnityEngine;

namespace RemixSurvivors
{
    [CreateAssetMenu(menuName = "RemixSurvivors.Entity/Action")]
    public class EntityAction : AIAction
    {
        [SerializeField] EntityState _state;
        public EntityState State => _state;

        public override void Execute(Context context) { }
    }
}
