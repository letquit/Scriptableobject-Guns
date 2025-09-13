using UnityEngine;

/// <summary>
/// 冰霜效果类，继承自AbstractAreaOfEffect，用于处理冰霜法术的区域效果
/// </summary>
/// <remarks>
/// 该类在基础区域效果的基础上增加了减速效果，能够对范围内的敌人施加减速状态
/// </remarks>
public class Frost : AbstractAreaOfEffect
{
    /// <summary>
    /// 减速衰减曲线，定义减速效果随时间的变化规律
    /// </summary>
    public AnimationCurve SlowDecay;

    /// <summary>
    /// 构造函数，初始化冰霜效果的基本参数
    /// </summary>
    /// <param name="Radius">效果半径</param>
    /// <param name="DamageFalloff">伤害衰减曲线</param>
    /// <param name="BaseDamage">基础伤害值</param>
    /// <param name="MaxEnemiesAffected">最大影响敌人数量</param>
    public Frost(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected)
        : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected)
    {
        this.SlowDecay = new AnimationCurve();
    }

    /// <summary>
    /// 构造函数，初始化冰霜效果的所有参数，包括自定义减速衰减曲线
    /// </summary>
    /// <param name="Radius">效果半径</param>
    /// <param name="DamageFalloff">伤害衰减曲线</param>
    /// <param name="BaseDamage">基础伤害值</param>
    /// <param name="MaxEnemiesAffected">最大影响敌人数量</param>
    /// <param name="SlowDecay">减速衰减曲线</param>
    public Frost(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected,
        AnimationCurve SlowDecay) 
        : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected)
    {
        this.SlowDecay = SlowDecay;
    }
    
    /// <summary>
    /// 处理碰撞影响，在基础碰撞处理的基础上增加减速效果
    /// </summary>
    /// <param name="ImpactedObject">被碰撞的对象</param>
    /// <param name="HitPosition">碰撞位置</param>
    /// <param name="HitNormal">碰撞法线</param>
    /// <param name="Gun">武器脚本对象</param>
    public override void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, GunScriptableObject Gun)
    {
        // 调用基类的碰撞处理方法
        base.HandleImpact(ImpactedObject, HitPosition, HitNormal, Gun);

        // 遍历所有受击对象，对可减速的目标施加减速效果
        for (int i = 0; i < Hits; i++)
        {
            if (HitObjects[i].TryGetComponent(out ISlowable slowable))
            {
                slowable.Slow(SlowDecay);
            }
        }
    }
}

