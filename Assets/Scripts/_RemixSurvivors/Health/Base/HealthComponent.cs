using UnityEngine;
using UnityEngine.Events;

namespace RemixSurvivors.HealthSystem
{
    /// <summary>
    /// Abstract base class of the health component
    /// </summary>
    public abstract class HealthComponent
    {
        public abstract float CurrentHealth { get; set; }
        public abstract bool IsAlive { get; }
        public abstract float MaxHealth { get; set; }
        public UnityAction<float> OnHealthChanged { get; set; }
        public UnityAction OnHealthEmpty { get; set; }

        public abstract void AddHealth(float amount);
        public abstract void ApplyDamage(float damage);
        public abstract void SetHealth(float health);
    }
}
