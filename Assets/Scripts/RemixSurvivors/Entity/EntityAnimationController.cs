using System;
using UnityEngine;

namespace RemixSurvivors
{
        [Serializable]
        public class EntityAnimationController : MonoBehaviour
        {
            [SerializeField]
            Animator _animator;



            public void Awake() => _animator = GetComponentInChildren<Animator>();

            public void PlayStateAnimation(EntityState state)
            {
                switch (state)
                {
                    case EntityState.IDLE:
                        _animator.SetFloat("Speed", 0f);
                        break;
                    case EntityState.WALK:
                        _animator.SetFloat("Speed", 0.5f);
                        break;
                    case EntityState.CHASE:
                        _animator.Play("Locomotion");
                        _animator.SetFloat("Speed", 1f);
                        break;
                    case EntityState.ATTACK:
                        _animator.Play("Attack");
                        break;
                    case EntityState.DAMAGE:
                        int random = UnityEngine.Random.Range(0, 2);
                        _animator.Play($"Damage2");
                        break;
                    case EntityState.DEAD:
                        _animator.Play("Damage2");
                        break;
                }
            }
        }
}
