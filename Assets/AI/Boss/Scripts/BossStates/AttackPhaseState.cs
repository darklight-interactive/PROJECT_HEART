using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace ProjectHeart
{
	public class AttackPhaseState
	{
		private readonly Queue<IState> queue = new();

		private readonly List<Func<BossStateMachine, IState>> stateFactories = new();

		[SerializeField] private IState previousState;
		[SerializeField] private IState currentState;

		private BossStateMachine boss;

		private int remainingLoops;
		private bool infiniteLoop;

		public AttackPhaseState(BossStateMachine boss, int repeatingLoop = 1, bool runInfinitely = true)
		{
			this.boss = boss;
			this.remainingLoops = repeatingLoop;
			this.infiniteLoop = runInfinitely;

			stateFactories.Add(b => new RumbleState(b));
			stateFactories.Add(b => new JumpState(b));
			// stateFactories.Add(b => new FollowPlayerState(b));
			stateFactories.Add(b => new DropState(b));

			UpdateQueue();
		}

		private void UpdateQueue()
		{
			queue.Clear();
			foreach (var factory in stateFactories) {
				queue.Enqueue(factory(this.boss));
			 }
			// queue.Enqueue(new RumbleState(boss));
			// queue.Enqueue(new JumpState(boss));
			// queue.Enqueue(new FollowPlayerState(boss));
			// queue.Enqueue(new DropState(boss));
		}

		public void Tick()
		{

			if (this.currentState == null && queue.Count > 0)
			{
				this.currentState = queue.Dequeue();
				this.currentState.Enter();
			}

			this.currentState?.Tick();

			if (this.currentState?.IsComplete == true)
			{
				this.currentState.Exit();
				this.currentState = null;

				if (queue.Count == 0 && (infiniteLoop || this.remainingLoops > 1))
				{
					if (!infiniteLoop)
						this.remainingLoops--;
					UpdateQueue();
				}
			}
		}

		public bool IsComplete => !infiniteLoop && remainingLoops <= 0 && currentState == null && queue.Count == 0;
	}

}

