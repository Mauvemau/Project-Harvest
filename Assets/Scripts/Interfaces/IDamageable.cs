public interface IDamageable {
    public void TakeDamage(float damage);
    public void SetCurrentHealth(float value);
    public void SetMaxHealth(float value);
    public void Heal(float value);
    public void Kill();
    public void Revive();
    public bool IsAlive();
}
