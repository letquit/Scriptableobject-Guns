using UnityEngine;

/// <summary>
/// 可受伤接口，定义了游戏对象受到伤害和死亡相关的基本功能
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public int CurrentHealth { get; }

    /// <summary>
    /// 获取最大生命值
    /// </summary>
    public int MaxHealth { get; }

    /// <summary>
    /// 受到伤害事件委托，当对象受到伤害时触发
    /// </summary>
    /// <param name="Damage">受到的伤害值</param>
    public delegate void TakeDamageEvent(int Damage);

    /// <summary>
    /// 受到伤害事件，在对象受到伤害时调用
    /// </summary>
    public event TakeDamageEvent OnTakeDamage;

    /// <summary>
    /// 死亡事件委托，当对象死亡时触发
    /// </summary>
    /// <param name="Position">死亡时的位置</param>
    public delegate void DeathEvent(Vector3 Position);

    /// <summary>
    /// 死亡事件，在对象死亡时调用
    /// </summary>
    public event DeathEvent OnDeath;

    /// <summary>
    /// 对象受到伤害的处理方法
    /// </summary>
    /// <param name="Damage">受到的伤害值</param>
    public void TakeDamage(int Damage);
}

