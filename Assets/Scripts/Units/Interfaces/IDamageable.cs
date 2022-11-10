public interface IDamageable
{
    int Health { get; set; }
    bool IsDying { get; set; }
    void TakeDamage(int amount);
}
