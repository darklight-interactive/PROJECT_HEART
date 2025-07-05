using System;
using Darklight.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using static ProjectHeart.ProjectHeart_Actions;

namespace ProjectHeart.Input
{
    [Serializable]
    public class InputActionMap_UI : InputActionMap, IUIActions
    {
        [SerializeField, ShowOnly]
        Vector2 _navigateValue;

        [SerializeField, ShowOnly]
        bool _submitValue;

        [SerializeField, ShowOnly]
        bool _cancelValue;

        [SerializeField, ShowOnly]
        bool _pointValue;

        [SerializeField, ShowOnly]
        bool _clickValue;

        [SerializeField, ShowOnly]
        bool _rightClickValue;

        [SerializeField, ShowOnly]
        bool _scrollWheelValue;

        [SerializeField, ShowOnly]
        bool _middleClickValue;

        [SerializeField]
        Vector3 _trackedDevicePositionValue;

        [SerializeField]
        Quaternion _trackedDeviceOrientationValue;

        public InputActionMap_UI(GlobalInputReader inputListener)
            : base(inputListener) { }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}
