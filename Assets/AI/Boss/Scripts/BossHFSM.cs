using System;
using UnityEngine;

namespace Darklight.Behaviour
{
	public enum BOSS_PHASES { START, PHASE_01, PHASE_02, PHASE_03, DEAD }

	public class BossHFSM : FiniteState<BOSS_PHASES>
	{
		// substate 

		// transtion

		public BossHFSM(BOSS_PHASES stateType) : base(stateType)
		{

		}

		// substate is class the inherits from finite state machine 
	}
}