using UnityEngine;

/// <summary>
/// 爆炸效果类，继承自AbstractAreaOfEffect抽象类
/// 用于处理爆炸范围伤害效果
/// </summary>
/// <param name="Radius">爆炸半径范围</param>
/// <param name="DamageFalloff">伤害衰减曲线，定义距离爆炸中心越远伤害越低的规律</param>
/// <param name="BaseDamage">基础伤害值</param>
/// <param name="MaxEnemiesAffected">最大影响敌人数量</param>
public class Explode : AbstractAreaOfEffect
{
    /// <summary>
    /// 构造函数，初始化爆炸效果的各项参数
    /// </summary>
    /// <param name="Radius">爆炸作用半径</param>
    /// <param name="DamageFalloff">伤害随距离衰减的曲线</param>
    /// <param name="BaseDamage">爆炸的基础伤害值</param>
    /// <param name="MaxEnemiesAffected">最多能影响的敌人数量</param>
    public Explode(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected) : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected) { }
}

