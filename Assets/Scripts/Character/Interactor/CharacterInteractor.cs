using Darklight.Behaviour;
using ProjectHeart.Input;
using UnityEngine;

namespace ProjectHeart
{
    public class CharacterInteractor : Interactor
    {
        void Start()
        {
            GlobalInputReader.PlayerInput.Interact += HandleInteractInput;
        }

        void OnDisable()
        {
            GlobalInputReader.PlayerInput.Interact -= HandleInteractInput;
        }

        void HandleInteractInput()
        {
            InteractWithTarget();
        }
    }
}
