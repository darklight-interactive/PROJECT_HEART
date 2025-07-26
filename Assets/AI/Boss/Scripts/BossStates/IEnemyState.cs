using UnityEngine;

namespace ProjectHeart
{
	#region IState
	public interface IState
	{
		void Enter();
		void Tick();
		bool IsComplete { get; }
		void Exit();
	}
	#endregion

	#region RumbleState
	public class RumbleState : IState
	{
		private Animator animator;
		private bool triggered;

		private static readonly int rumbleAnimation = Animator.StringToHash("Rumble_00");
		private static readonly int layerIndex = 0;
		private static readonly float crossFadeDuration = 0;

		public bool IsComplete { get; private set; }

		public RumbleState(BossStateMachine bossStateMachine)
		{
			this.animator = bossStateMachine.Animator;


		}

		public void Enter()
		{
			Debug.Log("Entering the RumbleState");
			animator.CrossFade(rumbleAnimation, crossFadeDuration, layerIndex);
			triggered = true;
		}

		public void Tick()
		{
			if (!triggered) return;

			var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
			if (state.normalizedTime >= 1f)
			{
				IsComplete = true;
			}
		}

		public void Exit() { }
	}
	#endregion

	#region JumpState
	public class JumpState : IState
	{
		private Animator animator;
		private bool triggered;

		private static readonly int jumpAnimation = Animator.StringToHash("Jump_00");
		private static readonly int layerIndex = 0;
		private static readonly float crossFadeDuration = 0;

		public bool IsComplete { get; private set; }

		public JumpState(BossStateMachine bossStateMachine)
		{
			this.animator = bossStateMachine.Animator;
		}

		public void Enter()
		{
			Debug.Log("Entering the JumpState");

			animator.CrossFade(jumpAnimation, crossFadeDuration, layerIndex);
			triggered = true;
		}

		public void Tick()
		{
			if (!triggered) return;

			var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
			if (state.normalizedTime >= 1f)
			{
				IsComplete = true;
			}
		}

		public void Exit() { }
	}
	#endregion

	#region FollowPlayerState
	public class FollowPlayerState : IState
	{
		private Animator animator;
		private Transform bossTransform;
		private Transform player;

		private float proximityThreshold = 3.5f;
		private float speed;
		private float rotationSpeed;

		public bool IsComplete { get; private set; }

		public FollowPlayerState(BossStateMachine bossStateMachine)
		{
			this.animator = bossStateMachine.Animator;
			this.bossTransform = bossStateMachine.transform;
			this.player = bossStateMachine.Player.transform;
			this.speed = bossStateMachine.BossStats.Speed;
			this.rotationSpeed = bossStateMachine.BossStats.Speed / 2f;
		}

		public void Enter()
		{
			Debug.Log("Entering the FollowPlayer State");
			// animator.applyRootMotion = true;
		}

		public void Tick()
		{
			Vector3 direction = (player.position - bossTransform.position).normalized;

			bossTransform.position = Vector3.MoveTowards(bossTransform.position, player.position, speed * Time.deltaTime);

			Quaternion lookRotation = Quaternion.LookRotation(direction);
			bossTransform.rotation = Quaternion.RotateTowards(bossTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

			if (Vector3.Distance(bossTransform.position, player.position) < proximityThreshold)
			{
				IsComplete = true;
			}
		}

		public void Exit()
		{
			//animator.applyRootMotion = false;
		}
	}
	#endregion

	#region DropState
	public class DropState : IState
	{
		private Animator animator;
		private bool triggered;

		private static readonly int dropAnimation = Animator.StringToHash("Drop_00");
		private static readonly int layerIndex = 0;
		private static readonly float crossFadeDuration = 0;

		public bool IsComplete { get; private set; }

		public DropState(BossStateMachine bossStateMachine)
		{
			this.animator = bossStateMachine.Animator;
		}

		public void Enter()
		{
			Debug.Log("Entering the DropState");

			animator.CrossFade(dropAnimation, crossFadeDuration, layerIndex);
			triggered = true;
		}

		public void Tick()
		{
			if (!triggered) return;

			var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
			if (state.normalizedTime >= 1f)
			{
				IsComplete = true;
			}
		}

		public void Exit() { }
	}
	#endregion
}
