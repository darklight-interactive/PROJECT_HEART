using AYellowpaper.SerializedCollections;
using Darklight.Core3D;
using UnityEngine;

namespace ProjectHeart.Character
{
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "CharacterAnimatorSettings",
        menuName = "ProjectHeart/Character/AnimatorSettings"
    )]
    public class CharacterAnimatonSettings : ScriptableObject
    {
        [SerializeField]
        AnimationClip _idleAnimation;

        [SerializeField]
        SerializedDictionary<Spatial3D.Direction, AnimationClip> _walkAnimations =
            new SerializedDictionary<Spatial3D.Direction, AnimationClip>()
            {
                { Spatial3D.Direction.FORWARD, null },
                { Spatial3D.Direction.BACKWARD, null },
                { Spatial3D.Direction.LEFT, null },
                { Spatial3D.Direction.RIGHT, null }
            };

        [SerializeField]
        SerializedDictionary<Spatial3D.Direction, AnimationClip> _runAnimations =
            new SerializedDictionary<Spatial3D.Direction, AnimationClip>()
            {
                { Spatial3D.Direction.FORWARD, null },
                { Spatial3D.Direction.BACKWARD, null },
                { Spatial3D.Direction.LEFT, null },
                { Spatial3D.Direction.RIGHT, null }
            };

        public AnimationClip IdleAnimation => _idleAnimation;
        public SerializedDictionary<Spatial3D.Direction, AnimationClip> WalkAnimations =>
            _walkAnimations;
        public SerializedDictionary<Spatial3D.Direction, AnimationClip> RunAnimations =>
            _runAnimations;
    }
}
