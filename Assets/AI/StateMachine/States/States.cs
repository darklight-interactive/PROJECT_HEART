namespace ProjectHeart
{
	public abstract class State
	{
		protected StateMachine sc;
		protected float duration;

		public void OnStateEnter(StateMachine stateMachine)
		{
			// Code placed here will always run
			sc = stateMachine;
			OnEnter();
		}

		protected virtual void OnEnter()
		{
			// Code placed here can be overridden
		}

		public void OnStateUpdate()
		{
			// Code placed here will always run
			OnUpdate();
		}

		protected virtual void OnUpdate()
		{
			// Code placed here can be overridden
		}

		public void OnStateHurt()
		{
			// Code placed here will always run
			OnHurt();
		}

		protected virtual void OnHurt()
		{
			// Code placed here can be overridden
		}

		public void OnStateExit()
		{
			// Code placed here will always run
			OnExit();
		}

		protected virtual void OnExit()
		{
			// Code placed here can be overridden
		}
	}

}
