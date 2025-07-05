using System;
using Darklight.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using static ProjectHeart.ProjectHeart_Actions;

namespace ProjectHeart.Input
{
    [Serializable]
    public class InputActionMap_Player : InputActionMap, IPlayerActions
    {
        [SerializeField, ShowOnly]
        Vector2 _moveValue;

        [SerializeField, ShowOnly]
        Vector2 _lookValue;

        [SerializeField, ShowOnly]
        bool _attackValue;

        [SerializeField, ShowOnly]
        bool _interactValue;

        [SerializeField, ShowOnly]
        bool _crouchValue;

        [SerializeField, ShowOnly]
        bool _jumpValue;

        [SerializeField, ShowOnly]
        bool _previousValue;

        [SerializeField, ShowOnly]
        bool _nextValue;

        public Vector2 MoveValue => _moveValue;
        public bool IsMoveActive => _moveValue != Vector2.zero;
        public Vector2 LookValue => _lookValue;
        public bool AttackValue => _attackValue;
        public bool InteractValue => _interactValue;
        public bool CrouchValue => _crouchValue;
        public bool JumpValue => _jumpValue;
        public bool PreviousValue => _previousValue;

        /// <summary>
        /// Called when the player moves.
        /// </summary>
        public event Action<Vector2> Move;

        /// <summary>
        /// Called when the player cancels the move.
        /// </summary>
        public event Action MoveCancelled;

        /// <summary>
        /// Called when the player looks.
        /// <param name="delta">The delta of the look.</param>
        /// <param name="isMouse">Whether the look is from the mouse.</param>
        /// </summary>
        public event Action<Vector2, bool> Look;

        /// <summary>
        /// Called when the player attacks.
        /// </summary>
        public event Action Attack;

        /// <summary>
        /// Called when the player interacts.
        /// </summary>
        public event Action Interact;

        /// <summary>
        /// Called when the player crouches.
        /// </summary>
        public event Action Crouch;

        /// <summary>
        /// Called when the player jumps.
        /// </summary>
        public event Action Jump;

        /// <summary>
        /// Called when the player goes to the previous item.
        /// </summary>
        public event Action Previous;

        /// <summary>
        /// Called when the player goes to the next item.
        /// </summary>
        public event Action Next;

        public InputActionMap_Player(GlobalInputReader inputListener)
            : base(inputListener) { }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _moveValue = context.ReadValue<Vector2>();
                Move?.Invoke(_moveValue);
            }
            else if (context.canceled)
            {
                _moveValue = Vector2.zero;
                MoveCancelled?.Invoke();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                _lookValue = context.ReadValue<Vector2>();
                Look?.Invoke(_lookValue, GlobalInputReader.IsMouse(context));
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                Attack?.Invoke();
            }

            if (context.started || context.performed)
                _attackValue = true;
            else if (context.canceled)
                _attackValue = false;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                Interact?.Invoke();
            }

            if (context.started || context.performed)
                _interactValue = true;
            else if (context.canceled)
                _interactValue = false;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                Crouch?.Invoke();
            }

            if (context.started || context.performed)
                _crouchValue = true;
            else if (context.canceled)
                _crouchValue = false;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                Jump?.Invoke();
            }

            if (context.started || context.performed)
                _jumpValue = true;
            else if (context.canceled)
                _jumpValue = false;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
