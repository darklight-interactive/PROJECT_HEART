using UnityEngine.Events;

namespace ProjectHeart.Interaction
{
    /// <summary>
    /// A simple interaction reciever that toggles an 'ON' and 'OFF' UnityEvent when the interactable is interacted with.
    /// </summary>
    public class OneShotInteractionReciever : BaseInteractionReciever
    {
        public UnityEvent OnInteraction;

        public override void OnAcceptTarget() { }

        public override void OnAcceptInteraction()
        {
            OnInteraction.Invoke();
        }

        public override void OnReset() { }
    }
}
