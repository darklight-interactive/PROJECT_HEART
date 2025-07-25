using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace ProjectHeart
{
	public class AttackPhaseState
	{
		private readonly Queue<IState> queue = new();

		[SerializeField] private IState previousState;
		[SerializeField] private IState currentState;

		public AttackPhaseState(BossStateMachine boss)
		{
		
			queue.Enqueue(new RumbleState(boss));
			queue.Enqueue(new JumpState(boss));
			queue.Enqueue(new FollowPlayerState(boss));
			queue.Enqueue(new DropState(boss));
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
			}
		}

		public bool IsComplete => currentState == null && queue.Count == 0;
	}

}

