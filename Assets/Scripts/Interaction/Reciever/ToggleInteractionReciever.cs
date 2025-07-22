using Darklight.Behaviour;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectHeart.Interaction
{
    /// <summary>
    /// A simple interaction reciever that toggles an 'ON' and 'OFF' UnityEvent when the interactable is interacted with.
    /// </summary>
    public class ToggleInteractionReciever : BaseInteractionReciever
    {
        public override InteractionType InteractionType => InteractionType.TOGGLE;

        [SerializeField, ReadOnly]
        bool _isOn = false;

        public UnityEvent OnEvent;
        public UnityEvent OffEvent;

        public override void OnAcceptTarget()
        {
            Debug.Log("SimpleInteractionReciever :: OnAcceptTarget", this);
        }

        public override void OnAcceptInteraction()
        {
            Debug.Log("SimpleInteractionReciever :: OnAcceptInteraction", this);
            if (_isOn)
            {
                OffEvent.Invoke();
                _isOn = false;
            }
            else
            {
                OnEvent.Invoke();
                _isOn = true;
            }
        }

        public override void OnReset()
        {
            Debug.Log("SimpleInteractionReciever :: OnReset", this);
            _isOn = false;
            OffEvent.Invoke();
        }
    }
}
