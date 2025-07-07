using Darklight.Behaviour;

namespace ProjectHeart.Interaction
{
    public enum InteractionType
    {
        SIMPLE,
        TOGGLE,
        TARGET,
        DIALOGUE,
        CHOICE,
        DESTINATION
    }

    public abstract class BaseInteractionReciever
        : InteractionReciever<BaseInteractableData_SO, InteractionType> { }
}
