using System;
using UnityEngine;

/// <summary>
/// 枪械修改器应用器类，用于在游戏开始时对玩家当前装备的枪械应用预设的属性修改器
/// </summary>
public class GunModifierApplier : MonoBehaviour
{
    [SerializeField] 
    private ImpactType ImpactTypeOverride;
    [SerializeField]
    private PlayerGunSelector GunSelector;

    /// <summary>
    /// 在游戏对象启动时执行，应用枪械属性修改器到当前激活的枪械
    /// </summary>
    private void Start()
    {
        // 延迟一帧执行，确保PlayerGunSelector.Start()已经执行完毕
        Invoke(nameof(ApplyModifiers), 0.1f);
        
        // // 创建并应用伤害修改器，将当前枪械的伤害提升1.5倍
        // DamageModifier damageModifier = new()
        // {
        //     Amount = 1.5f,
        //     AttributeName = "DamageConfig/DamageCurve"
        // };
        // damageModifier.Apply(GunSelector.ActiveGun);
        //
        // // 创建并应用散布修改器，将当前枪械的散布设置为零
        // Vector3Modifier spreadModifier = new()
        // {
        //     Amount = Vector3.zero,
        //     AttributeName = "ShootConfig/Spread"
        // };
        // spreadModifier.Apply(GunSelector.ActiveGun);
    }
    
    /// <summary>
    /// 应用所有预设的枪械修改器到当前激活的枪械上
    /// 包括冲击类型修改和子弹撞击效果的替换
    /// </summary>
    private void ApplyModifiers()
    {
        // 应用冲击类型修改器，使用序列化字段中指定的冲击类型覆盖默认值
        new ImpactTypeModifier()
        {
            Amount = ImpactTypeOverride
        }.Apply(GunSelector.ActiveGun);

        // 替换当前枪械的子弹撞击效果为冰冻效果，并设置相关参数
        GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
        {
            new Frost(
                1.5f,
                new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.25f)}),
                10,
                10,
                new AnimationCurve(new Keyframe[] { 
                    new Keyframe(0, 0.25f),
                    new Keyframe(1.75f, 0.25f),
                    new Keyframe(2, 1)
                })
            )
        };
        
        // GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
        // {
        //     new Explode(
        //         1.5f,
        //         new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.25f)}),
        //         10,
        //         10
        //     )
        // };
    }
}

