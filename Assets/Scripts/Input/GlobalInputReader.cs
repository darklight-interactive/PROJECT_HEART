using System;
using Darklight.Behaviour;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ProjectHeart.Input
{
    public class GlobalInputReader : MonoBehaviourSingleton<GlobalInputReader>
    {
        ProjectHeart_Actions _inputActions;

        [SerializeField]
        InputActionMap_Player _playerActionMap;

        [SerializeField]
        InputActionMap_UI _uiActionMap;

        public static InputActionMap_Player PlayerInput => Instance._playerActionMap;
        public static InputActionMap_UI UIInput => Instance._uiActionMap;

        public static bool IsMouse(InputAction.CallbackContext context) =>
            context.control.device is Mouse;

        public void OnEnable() => Initialize();

        public void OnDisable() => DisableAllActions();

        public void OnDestroy() => _inputActions?.Dispose();

        public void EnablePlayerActions()
        {
            _inputActions.Player.Enable();
            _inputActions.UI.Disable();
        }

        public void EnableUIActions()
        {
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
        }

        public void DisableAllActions()
        {
            _inputActions.Player.Disable();
            _inputActions.UI.Disable();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Debug.Log("GlobalInputReader Initialize");

            // Create the input action classes
            if (_inputActions == null)
            {
                _inputActions = new ProjectHeart_Actions();
                _playerActionMap = new InputActionMap_Player(this);
                _uiActionMap = new InputActionMap_UI(this);

                // Assign the callbacks to the input action classes
                _inputActions.Player.SetCallbacks(_playerActionMap);
                _inputActions.UI.SetCallbacks(_uiActionMap);
            }

            // Enable the player actions by default in play mode
            if (Application.isPlaying)
                EnablePlayerActions();
        }
    }
}
