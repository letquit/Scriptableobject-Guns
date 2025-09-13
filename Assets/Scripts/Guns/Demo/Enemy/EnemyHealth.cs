using UnityEngine;

/// <summary>
/// 敌人生命值管理类，实现IDamageable接口来处理伤害和死亡逻辑
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _Health;
    [SerializeField]
    private int _MaxHealth = 100;
    public int CurrentHealth { get => _Health; private set => _Health = value; }
    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    /// <summary>
    /// 当对象启用时调用，初始化当前生命值为最大生命值
    /// </summary>
    private void OnEnable()
    {
        _Health = MaxHealth;
    }

    /// <summary>
    /// 处理受到的伤害，减少生命值并触发相应事件
    /// </summary>
    /// <param name="Damage">受到的伤害值</param>
    public void TakeDamage(int Damage)
    {
        // 确保伤害不超过当前生命值
        int damageTaken = Damage;
        if (CurrentHealth < damageTaken)
        {
            damageTaken = CurrentHealth;
        }

        CurrentHealth -= damageTaken;
        
        // 如果受到了伤害，则触发受伤事件
        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        // 如果生命值降为0且受到了伤害，则触发死亡事件
        if (CurrentHealth == 0 && damageTaken != 0)
        {
            // 检查对象的Tag是否为"Box"
            if (CompareTag("Box"))
            {
                // 如果是Box标签，则延迟5秒后销毁
                Destroy(gameObject);
            }
            
            OnDeath?.Invoke(transform.position);
        }
    }
}
