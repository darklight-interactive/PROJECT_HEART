namespace ProjectHeart.Input
{
    public abstract class InputActionMap
    {
        protected GlobalInputReader _inputListener;

        protected InputActionMap(GlobalInputReader inputListener)
        {
            _inputListener = inputListener;
        }
    }
}
