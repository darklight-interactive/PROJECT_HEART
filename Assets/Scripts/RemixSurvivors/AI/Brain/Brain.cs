using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Collection;
using NaughtyAttributes;
using RemixSurvivors.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

namespace RemixSurvivors.AI
{
    public abstract class Brain : MonoBehaviour
    {
        public abstract List<AIAction> Actions { get; }
        public abstract Context Context { get; }        
        public abstract void Initialize();
        public abstract void UpdateContextData();
        public abstract AIAction GetBestAction();

        protected void Awake() => Initialize();

        protected virtual void Update()
        {
            UpdateContextData();

            AIAction bestAction = GetBestAction();
            bestAction?.Execute(Context);
        }
    }
}
