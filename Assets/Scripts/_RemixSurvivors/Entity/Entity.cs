using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.Behaviour;
using Darklight.Collection;
using NaughtyAttributes;
using RemixSurvivors.AI;
using RemixSurvivors.HealthSystem;
using UnityEngine;

namespace RemixSurvivors
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public enum EntityState
    {
        IDLE,
        WALK,
        CHASE,
        ATTACK,
        DAMAGE,
        DEAD
    }

    [RequireComponent(typeof(EntityAnimationController))]
    public partial class Entity : Brain
    {
        [SerializeField]
        Rigidbody _rigidbody;

        [SerializeField]
        Transform _model;

        [SerializeField]
        Context _context;

        [SerializeField]
        int _damage = 10;

        [SerializeField]
        Vector2 _knockbackForce = new Vector2(50f, 100f);

        [SerializeField]
        EntityStateMachine _stateMachine;

        [SerializeField]
        EntityAnimationController _animationController;

        [SerializeField]
        EntityHealthController _healthController;

        public override List<AIAction> Actions => _stateMachine.Actions.Cast<AIAction>().ToList();
        public override Context Context => _context;

        public Rigidbody Rigidbody => _rigidbody;
        public EntityStateMachine StateMachine => _stateMachine;
        public EntityAnimationController AnimationController => _animationController;
        public EntityHealthController HealthController => _healthController;

        public override void Initialize()
        {
            _context = new Context(this);
            _stateMachine = GetComponentInChildren<EntityStateMachine>();
            _animationController = GetComponentInChildren<EntityAnimationController>();
            _healthController = GetComponentInChildren<EntityHealthController>();
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }

        public override AIAction GetBestAction()
        {
            return _stateMachine.GetBestAction();
        }

        public override void UpdateContextData()
        {
            Context.SetData("health", HealthController.CurrentHealth);
            Context.SetData("damage", _damage);
        }

        protected override void Update()
        {
            UpdateContextData();

            EntityAction bestAction = GetBestAction() as EntityAction;
            bestAction?.Execute(Context);

            EntityState bestActionState = bestAction.State;
            _stateMachine.TryGoToState(bestActionState);
        }

        public void RotateModelToLookAt(Transform target)
        {
            Vector3 direction = (target.position - _model.position).normalized;
            _model.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        public void Knockback(Vector3 direction)
        {
            Vector3 combinedForce = new Vector3(
                _knockbackForce.x * direction.x,
                _knockbackForce.y,
                _knockbackForce.x * direction.z
            );
            _rigidbody.AddForce(combinedForce, ForceMode.Impulse);
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Entity))]
        public class EntityCustomEditor : BrainCustomEditor
        {
            SerializedObject _serializedObject;
            Entity _script;

            public override void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Entity)target;
                _script.Initialize();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Initialize"))
                {
                    _script.Initialize();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }
}
