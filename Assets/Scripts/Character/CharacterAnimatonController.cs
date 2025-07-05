using Darklight.Behaviour;
using Darklight.Core3D;
using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;
using Direction = Darklight.Core3D.Spatial3D.Direction;

namespace ProjectHeart.Character
{
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterAnimator : ManualAnimator
    {
        CharacterMovementController _movement => GetComponent<CharacterMovementController>();

        [SerializeField]
        Animator _animator;

        [SerializeField, Expandable]
        [CreateAsset("NewCharacterAnimatonSettings", "Assets/Resources/Character")]
        CharacterAnimatonSettings _settings;

        public override void DefaultAnimation(int layer)
        {
            MovementState state = _movement.StateMachine.CurrentState;
            switch (state)
            {
                case MovementState.IDLE:
                    Play(new AnimationData(Animations.IDLE, false, null, 0.25f));
                    break;
                case MovementState.GROUND_MOVE:
                    Play(new AnimationData(Animations.WALK, false, null, 0.25f));
                    break;
            }
        }

        void Start()
        {
            Initialize(_animator);
        }

        void Update()
        {
            DefaultAnimation(0);
        }
    }
}
