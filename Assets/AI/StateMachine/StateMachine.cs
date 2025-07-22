using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectHeart
{
    public abstract class StateMachine : MonoBehaviour
    {

        protected State currentState { get; set; }


        public virtual void ChangeToNextState(State newState)
        {

        }

        public virtual void OnGameStateChanged()
        {

        }

    }

    
}
