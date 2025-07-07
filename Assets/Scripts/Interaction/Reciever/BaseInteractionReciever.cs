using Darklight.Behaviour;

namespace ProjectHeart.Interaction
{
    public enum InteractionType
    {
        ONE_SHOT,
        TOGGLE,
        TARGET,
        DIALOGUE,
        CHOICE,
        DESTINATION
    }

    public abstract class BaseInteractionReciever
        : InteractionReciever<BaseInteractableData_SO, InteractionType> { }
}
