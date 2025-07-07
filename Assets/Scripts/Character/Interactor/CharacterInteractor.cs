using Darklight.Behaviour;
using ProjectHeart.Input;
using UnityEngine;

namespace ProjectHeart
{
    public class CharacterInteractor : Interactor
    {
        void Start()
        {
            if (GlobalInputReader.Instance != null)
                GlobalInputReader.PlayerInput.Interact += HandleInteractInput;
            else
                Debug.LogError("CharacterInteractor :: GlobalInputReader could not be found", this);
        }

        void OnDestroy()
        {
            if (GlobalInputReader.Instance != null)
                GlobalInputReader.PlayerInput.Interact -= HandleInteractInput;
        }

        void HandleInteractInput()
        {
            InteractWithTarget();
        }
    }
}
