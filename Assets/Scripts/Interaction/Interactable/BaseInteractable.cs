using System;
using System.Collections.Generic;
using Darklight.Behaviour;
using Darklight.Collections;
using Darklight.Editor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;

#endif

namespace ProjectHeart.Interaction
{
    public partial class BaseInteractable
        : Interactable<BaseInteractableData_SO, InteractionType>,
            IInteractable,
            IUnityEditorListener
    {
        public enum State
        {
            NULL,
            READY,
            TARGET,
            START,
            CONTINUE,
            COMPLETE,
            DISABLED
        }

        protected readonly List<State> VALID_INTERACTION_STATES = new List<State>
        {
            State.TARGET,
            State.START,
            State.CONTINUE
        };

        [SerializeField]
        InternalStateMachine _stateMachine;

        public override Collider Collider => GetComponent<Collider>();

        public InternalStateMachine StateMachine => _stateMachine;
        public State CurrentState => _stateMachine.CurrentState;

        public override Action OnAcceptTarget { get; set; }
        public override Action OnAcceptInteraction { get; set; }
        public override Action OnReset { get; set; }

        #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
        [Button]
        public override void Initialize()
        {
            base.Initialize();

            // << SET TO READY STATE >> ------------------------------------
            _stateMachine = new InternalStateMachine(this);
            _stateMachine.GoToState(State.READY);
        }

        public override bool AcceptTarget(IInteractor interactor, bool force = false)
        {
            base.AcceptTarget(interactor, force);

            // If not forced, check to make sure the interactable is in a valid state
            if (!force)
            {
                // << CONFIRM VALIDITY >> ------------------------------------
                if (CurrentState != State.READY)
                    return false;
            }

            // << ACCEPT TARGET >> ------------------------------------
            StateMachine.GoToState(State.TARGET);
            return true;
        }

        public override bool AcceptInteraction(IInteractor interactor, bool force = false)
        {
            base.AcceptInteraction(interactor, force);

            if (!force)
            {
                // << CONFIRM VALIDITY >> ------------------------------------
                if (!VALID_INTERACTION_STATES.Contains(CurrentState))
                    return false;
            }

            // Update the state machine
            switch (CurrentState)
            {
                case State.START:
                case State.CONTINUE:
                    StateMachine.GoToState(State.CONTINUE, true);
                    break;
                case State.COMPLETE:
                case State.DISABLED:
                    break;
                default:
                    StateMachine.GoToState(State.START);
                    break;
            }

            return true;
        }

        public override void Refresh()
        {
            StateMachine.Step();
        }

        public override void Reset()
        {
            base.Reset();

            if (StateMachine == null)
                return;
            StateMachine.GoToState(State.READY);
        }
        #endregion

        protected override void OnDrawGizmos()
        {
            Vector3 labelPos = transform.position + (Vector3.up * 0.25f);
#if UNITY_EDITOR
            CustomGizmos.DrawLabel(
                CurrentState.ToString(),
                labelPos,
                new GUIStyle()
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState() { textColor = Color.white }
                }
            );
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(BaseInteractable))]
        public class BaseInteractableCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            BaseInteractable _script;

            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (BaseInteractable)target;
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button("Initialize"))
                    _script.Initialize();

                base.OnInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }
}
