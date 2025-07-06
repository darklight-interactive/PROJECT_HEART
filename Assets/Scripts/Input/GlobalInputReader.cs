using System;
using System.Collections.Generic;
using Darklight.Behaviour;
using Darklight.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace ProjectHeart.Input
{
    public class GlobalInputReader : MonoBehaviourSingleton<GlobalInputReader>
    {
        // -------------- [[ STATIC INPUT TYPE ]] -------------- >>
        public enum InputType
        {
            NULL,
            KEYBOARD,
            TOUCH,
            GAMEPAD
        }

        public static InputType DeviceInputType
        {
            get => Instance._deviceInputType;
            private set => Instance._deviceInputType = value;
        }

        ProjectHeart_Actions _inputActions;

        [SerializeField, ShowOnly]
        InputType _deviceInputType;

        [SerializeField, ShowOnly]
        private List<string> _connectedDevices;

        [Header("Input Settings")]
        [SerializeField]
        bool _lockCursorOnStart = true;

        [Header("Input Action Maps")]
        [SerializeField]
        InputActionMap_Player _playerActionMap;

        [SerializeField]
        InputActionMap_UI _uiActionMap;

        public static InputActionMap_Player PlayerInput => Instance._playerActionMap;
        public static InputActionMap_UI UIInput => Instance._uiActionMap;

        public static event Action OnInitialized;

        public static bool IsMouse(InputAction.CallbackContext context) =>
            context.control.device is Mouse;

        public void OnEnable()
        {
            Initialize();
        }

        public void OnDisable()
        {
            DisableAllActions();
            UnregisterInputListeners();
        }

        public void OnDestroy()
        {
            _inputActions?.Dispose();
        }

        void LoadAllConnectedDevices()
        {
            _connectedDevices = new List<string>();
            foreach (InputDevice device in InputSystem.devices)
            {
                _connectedDevices.Add(device.displayName);
            }
        }

        void RegisterInputListeners()
        {
            foreach (var map in _inputActions.asset.actionMaps)
            {
                foreach (var action in map.actions)
                {
                    action.performed += OnAnyInputPerformed;
                }
            }
        }

        void UnregisterInputListeners()
        {
            foreach (var map in _inputActions.asset.actionMaps)
            {
                foreach (var action in map.actions)
                {
                    action.performed -= OnAnyInputPerformed;
                }
            }
        }

        void OnAnyInputPerformed(InputAction.CallbackContext context)
        {
            var device = context.control.device;

            if (device is Mouse || device is Keyboard)
            {
                DeviceInputType = InputType.KEYBOARD;
            }
            else if (device is Gamepad)
            {
                DeviceInputType = InputType.GAMEPAD;
            }
            else if (device is Touchscreen)
            {
                DeviceInputType = InputType.TOUCH;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            Debug.Log($"{Prefix} Initialized");

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

            RegisterInputListeners();

            // Enable the player actions by default in play mode
            if (Application.isPlaying)
            {
                if (_lockCursorOnStart)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                EnablePlayerActions();
            }

            LoadAllConnectedDevices();
            OnInitialized?.Invoke();
        }

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
    }
}
