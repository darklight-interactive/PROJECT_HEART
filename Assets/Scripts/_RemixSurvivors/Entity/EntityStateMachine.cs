using System;
using System.Collections;
using System.Collections.Generic;
using Darklight.Behaviour;
using NaughtyAttributes;
using RemixSurvivors.AI;
using UnityEngine;

namespace RemixSurvivors
{
    public class EntityStateMachine : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        EntityState _currentState;

        [SerializeField, ReadOnly]
        bool _isDisabled = false;

        [HorizontalLine]
        [SerializeField, Expandable]
        List<EntityAction> _actions = new List<EntityAction>();

        [HorizontalLine]
        [SerializeField]
        private GameObject _damageParticles;

        [SerializeField]
        private GameObject _deadParticles;

        public Entity Entity => GetComponent<Entity>();
        public List<EntityAction> Actions => _actions;
        public Context Context => Entity.Context;

        public EntityState CurrentState => _currentState;

        StateMachine _stateMachine;

        public void Awake()
        {
            _stateMachine = new StateMachine(GetComponent<Entity>());
            foreach (EntityAction action in Actions)
            {
                switch (action.State)
                {
                    case EntityState.IDLE:
                        _stateMachine.AddState(new IdleState(Entity, action));
                        break;
                    case EntityState.CHASE:
                        _stateMachine.AddState(new ChaseState(Entity, action));
                        break;
                    case EntityState.ATTACK:
                        _stateMachine.AddState(new AttackState(Entity, action));
                        break;
                }
            }

            _stateMachine.AddState(new DamageState(Entity, null));
        }

        public void Update()
        {
            if (_stateMachine != null && _stateMachine.CurrentFiniteState != null)
                _currentState = _stateMachine.CurrentFiniteState.StateType;

            _stateMachine.Step();
        }

        public bool TryGoToState(EntityState state)
        {
            if (_isDisabled)
                return false;

            _stateMachine.GoToState(state);
            return true;
        }

        public void ForceGoToState(EntityState state)
        {
            _stateMachine.GoToState(state, true);
        }

        public void SetDisabledForSeconds(float seconds)
        {
            StartCoroutine(SetDisabledForSecondsCoroutine(seconds));
        }

        IEnumerator SetDisabledForSecondsCoroutine(float seconds)
        {
            _isDisabled = true;
            yield return new WaitForSeconds(seconds);
            _isDisabled = false;
        }

        public EntityAction GetBestAction()
        {
            float highestUtility = float.MinValue;
            EntityAction bestAction = null;

            foreach (EntityAction action in Actions)
            {
                float utility = action.CalculateUtility(Context);
                if (utility > highestUtility)
                {
                    highestUtility = utility;
                    bestAction = action;
                }
            }
            return bestAction;
        }

        [Button]
        public void PlayDamageParticles()
        {
            GameObject particles = Instantiate(
                _damageParticles,
                transform.position,
                Quaternion.identity
            );
            Destroy(particles, 2f);
        }

        [Button]
        public void PlayDeadParticles()
        {
            GameObject particles = Instantiate(
                _deadParticles,
                transform.position,
                Quaternion.identity
            );
            Destroy(particles, 2f);
        }

        [Serializable]
        public class StateMachine : FiniteStateMachine<EntityState>
        {
            Entity _entity;

            public StateMachine(Entity entity)
                : base()
            {
                _entity = entity;
            }
        }

        public abstract class BaseState : FiniteState<EntityState>
        {
            protected Entity entity;
            protected EntityAction action;

            public BaseState(EntityState stateType, Entity entity, EntityAction action)
                : base(stateType)
            {
                this.entity = entity;
                this.action = action;
            }
        }

        public class IdleState : BaseState
        {
            public IdleState(Entity entity, EntityAction action)
                : base(EntityState.IDLE, entity, action) { }

            public override void Enter()
            {
                entity.AnimationController.PlayStateAnimation(EntityState.IDLE);
            }
        }

        public class WalkState : BaseState
        {
            public WalkState(Entity entity, EntityAction action)
                : base(EntityState.WALK, entity, action) { }

            public override void Enter()
            {
                Debug.Log("Entering WalkState");
            }
        }

        public class ChaseState : BaseState
        {
            public ChaseState(Entity entity, EntityAction action)
                : base(EntityState.CHASE, entity, action) { }

            public override void Enter()
            {
                entity.Context.Agent.enabled = true;
                entity.Rigidbody.linearVelocity = Vector3.zero;
            }

            public override void Execute()
            {
                if (entity.Context.Sensor.GetClosestTarget(action.TargetTag) == null)
                    return;

                Transform target = entity.Context.Sensor.GetClosestTarget(action.TargetTag);
                entity.RotateModelToLookAt(target);

                if (entity.Context.Agent.isOnNavMesh)
                {
                    entity.Context.Agent.SetDestination(target.position);
                }

                entity.AnimationController.PlayStateAnimation(EntityState.CHASE);
            }
        }

        public class AttackState : BaseState
        {
            public AttackState(Entity entity, EntityAction action)
                : base(EntityState.ATTACK, entity, action) { }

            public override void Enter()
            {
                if (
                    entity
                        .Context.Sensor.GetClosestTarget(action.TargetTag)
                        .TryGetComponent(out SurvivorHealthController survivorHealthController)
                )
                {
                    survivorHealthController.TakeDamage(entity.Context.GetData<int>("damage"));
                }

                entity.AnimationController.PlayStateAnimation(EntityState.ATTACK);
                entity.StateMachine.SetDisabledForSeconds(2f);

                entity.StateMachine.ForceGoToState(EntityState.IDLE);
            }
        }

        public class DamageState : BaseState
        {
            public DamageState(Entity entity, EntityAction action)
                : base(EntityState.DAMAGE, entity, action) { }

            public override void Enter()
            {
                entity.StateMachine.PlayDamageParticles();
                entity.StateMachine.SetDisabledForSeconds(2f);

                entity.AnimationController.PlayStateAnimation(EntityState.DAMAGE);

                Transform target = entity.Context.Sensor.GetClosestTarget("Player");
                if (target != null)
                    entity.RotateModelToLookAt(target);
            }
        }

        public class DeadState : BaseState
        {
            public DeadState(Entity entity, EntityAction action)
                : base(EntityState.DEAD, entity, action) { }

            public override void Enter()
            {
                entity.StateMachine.PlayDeadParticles();

                entity.StateMachine.SetDisabledForSeconds(10f);
                entity.Context.Agent.enabled = false;

                entity.AnimationController.PlayStateAnimation(EntityState.DEAD);
                entity.Context.Agent.SetDestination(entity.transform.position);
            }
        }
    }
}
