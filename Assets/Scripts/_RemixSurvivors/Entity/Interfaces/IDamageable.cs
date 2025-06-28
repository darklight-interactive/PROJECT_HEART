namespace RemixSurvivors
{
    public interface IDamageable
    {
        public HealthSystem.Health Health { get; }
        public HealthSystem.HitboxComponent Hitbox { get; }
        public void ApplyHealing(float amount);
        public void ApplyDamage(float amount);
    }
}
