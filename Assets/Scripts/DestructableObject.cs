using UnityEngine;

/// <summary>
/// 可破坏物体类，继承自MonoBehaviour并实现IDamageable接口
/// 用于处理游戏中的可破坏物体的生命周期和伤害逻辑
/// </summary>
public class DestructableObject : MonoBehaviour, IDamageable
{
    [SerializeField]
    private ParticleSystem DestructionSystem;
    [SerializeField]
    private int _Health;
    [SerializeField]
    private int _MaxHealth = 25;
    public int CurrentHealth { get => _Health; private set => _Health = value; }
    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    /// <summary>
    /// 处理物体受到伤害的逻辑
    /// 当生命值降至0或以下时，播放销毁特效并销毁物体
    /// </summary>
    /// <param name="Damage">受到的伤害值</param>
    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        // 检查物体是否被摧毁
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            // 实例化销毁特效并销毁当前物体
            Instantiate(DestructionSystem, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
