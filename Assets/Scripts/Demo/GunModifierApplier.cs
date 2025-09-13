using System;
using UnityEngine;

/// <summary>
/// 枪械修改器应用器类，用于在游戏开始时对玩家当前装备的枪械应用预设的属性修改器
/// </summary>
public class GunModifierApplier : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;

    /// <summary>
    /// 在游戏对象启动时执行，应用枪械属性修改器到当前激活的枪械
    /// </summary>
    private void Start()
    {
        // 创建并应用伤害修改器，将当前枪械的伤害提升1.5倍
        DamageModifier damageModifier = new()
        {
            Amount = 1.5f,
            AttributeName = "DamageConfig/DamageCurve"
        };
        damageModifier.Apply(GunSelector.ActiveGun);
        
        // 创建并应用散布修改器，将当前枪械的散布设置为零
        Vector3Modifier spreadModifier = new()
        {
            Amount = Vector3.zero,
            AttributeName = "ShootConfig/Spread"
        };
        spreadModifier.Apply(GunSelector.ActiveGun);
    }
}

