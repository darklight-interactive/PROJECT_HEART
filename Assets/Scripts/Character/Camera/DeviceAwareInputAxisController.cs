using System;
using Darklight.Game;
using ProjectHeart.Input;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectHeart.Character
{
    /// <summary>
    /// Custom input axis controller that adjusts gain based on the detected device type.
    /// Extends CinemachineInputAxisController to provide device-specific sensitivity settings.
    /// </summary>
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Device Aware Input Axis Controller")]
    public class DeviceAwareInputAxisController : CinemachineInputAxisController
    {
        [Header("Device-Specific Sensitivity")]
        [SerializeField, Tooltip("Sensitivity multiplier for mouse/keyboard input")]
        private float _mouseKeyboardSensitivity = 1f;

        [SerializeField, Tooltip("Sensitivity multiplier for gamepad input")]
        private float _gamepadSensitivity = 100f;

        [SerializeField, Tooltip("Sensitivity multiplier for touch input")]
        private float _touchSensitivity = 50f;

        /// <summary>
        /// Override the default reader to provide device-specific gain adjustment.
        /// </summary>
        protected override void InitializeControllerDefaultsForAxis(
            in IInputAxisOwner.AxisDescriptor axis,
            Controller controller
        )
        {
            base.InitializeControllerDefaultsForAxis(axis, controller);

            // Override the ReadControlValueOverride to apply device-specific gain
            ReadControlValueOverride = ApplyDeviceSpecificGain;
        }

        /// <summary>
        /// Applies device-specific gain to the input value based on the current device type.
        /// </summary>
        /// <param name="action">The input action being read</param>
        /// <param name="hint">The axis hint (X or Y)</param>
        /// <param name="context">The controller context</param>
        /// <param name="defaultReader">The default reader function</param>
        /// <returns>The adjusted input value with device-specific gain applied</returns>
        private float ApplyDeviceSpecificGain(
            InputAction action,
            IInputAxisOwner.AxisDescriptor.Hints hint,
            UnityEngine.Object context,
            Reader.ControlValueReader defaultReader
        )
        {
            // Get the base input value using the default reader
            float baseValue = defaultReader(action, hint, context, null);

            // Apply device-specific gain
            float deviceGain = GetDeviceSpecificGain();

            return baseValue * deviceGain;
        }

        /// <summary>
        /// Gets the appropriate sensitivity multiplier based on the current device type.
        /// </summary>
        /// <returns>The sensitivity multiplier for the current device type</returns>
        private float GetDeviceSpecificGain()
        {
            if (!Application.isPlaying || GlobalInputReader.Instance == null)
                return 1f;

            switch (GlobalInputReader.DeviceInputType)
            {
                case GlobalInputReader.InputType.KEYBOARD:
                    return _mouseKeyboardSensitivity;
                case GlobalInputReader.InputType.GAMEPAD:
                    return _gamepadSensitivity;
                case GlobalInputReader.InputType.TOUCH:
                    return _touchSensitivity;
                default:
                    return 1f;
            }
        }

        /// <summary>
        /// Called when the component is reset in the inspector.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            _mouseKeyboardSensitivity = 1f;
            _gamepadSensitivity = 100f;
            _touchSensitivity = 50f;
        }
    }
}
