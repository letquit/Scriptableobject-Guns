using UnityEngine;

/// <summary>
/// 抽象的范围效果类，实现碰撞处理接口，用于处理范围伤害效果
/// </summary>
public abstract class AbstractAreaOfEffect : ICollisionHandler
{
    public float Radius = 1f;
    public AnimationCurve DamageFalloff;
    public int BaseDamage = 10;
    public int MaxEnemiesAffected = 10;

    protected Collider[] HitObjects;
    protected int Hits;

    /// <summary>
    /// 构造函数，初始化范围效果的基本参数
    /// </summary>
    /// <param name="Radius">效果半径范围</param>
    /// <param name="DamageFalloff">伤害衰减曲线，用于计算距离对伤害的影响</param>
    /// <param name="BaseDamage">基础伤害值</param>
    /// <param name="MaxEnemiesAffected">最大影响敌人数量</param>
    public AbstractAreaOfEffect(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected)
    {
        this.Radius = Radius;
        this.DamageFalloff = DamageFalloff;
        this.BaseDamage = BaseDamage;
        this.MaxEnemiesAffected = MaxEnemiesAffected;
        HitObjects = new Collider[this.MaxEnemiesAffected];
    }

    /// <summary>
    /// 处理碰撞影响，在指定位置产生范围伤害效果
    /// </summary>
    /// <param name="ImpactedObject">被碰撞的对象</param>
    /// <param name="HitPosition">碰撞位置</param>
    /// <param name="HitNormal">碰撞法线方向</param>
    /// <param name="Gun">武器配置数据</param>
    public virtual void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, GunScriptableObject Gun)
    {
        // 检测范围内的所有碰撞体
        Hits = Physics.OverlapSphereNonAlloc(
            HitPosition,
            Radius,
            HitObjects,
            Gun.ShootConfig.HitMask
        );
        
        // 遍历检测到的碰撞体，对可受伤对象造成伤害
        for (int i = 0; i < Hits; i++)
        {
            if (HitObjects[i].TryGetComponent(out IDamageable damageable))
            {
                // 计算碰撞点到目标的距離，並根據距離計算傷害衰减
                float distance = Vector3.Distance(HitPosition, HitObjects[i].ClosestPoint(HitPosition));
                
                damageable.TakeDamage(
                    Mathf.CeilToInt(BaseDamage * DamageFalloff.Evaluate(distance / Radius))
                );
            }
        }
    }
}

