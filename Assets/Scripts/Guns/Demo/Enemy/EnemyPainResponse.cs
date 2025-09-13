using UnityEngine;

/// <summary>
/// 敌人受伤反应控制类，负责处理敌人受到伤害时的动画响应和死亡处理
/// 需要挂载到带有Animator组件的游戏对象上
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyPainResponse : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth Health;
    private Animator Animator;
    [SerializeField]
    [Range(1, 100)]
    private int MaxDamagePainThreshold = 5;

    /// <summary>
    /// 初始化组件引用
    /// 在Awake阶段获取Animator组件引用
    /// </summary>
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 处理敌人受伤的动画响应
    /// 根据受到的伤害值设置动画层权重，触发受伤动画
    /// </summary>
    /// <param name="Damage">受到的伤害值，用于计算动画层权重</param>
    public void HandlePain(int Damage)
    {
        if (Health.CurrentHealth != 0)
        {
            // 重置并重新触发受伤动画，根据伤害值与最大疼痛阈值的比例设置动画层权重
            Animator.ResetTrigger("Hit");
            Animator.SetLayerWeight(1, (float)Damage / MaxDamagePainThreshold);
            Animator.SetTrigger("Hit");
        }
    }

    /// <summary>
    /// 处理敌人死亡动画
    /// 启用根骨骼运动并触发死亡动画
    /// </summary>
    public void HandleDeath()
    {
        Animator.applyRootMotion = true;
        Animator.SetTrigger("Die"); 
    }
}
