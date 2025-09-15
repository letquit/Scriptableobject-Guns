using System;
using UnityEngine;

/// <summary>
/// 子弹穿透配置ScriptableObject类，用于定义子弹穿透行为的参数配置
/// 该类可以通过Unity编辑器的Assets/Create/Guns/Bullet Penetration Config菜单创建
/// </summary>
[CreateAssetMenu(fileName = "Bullet Penetration Config", menuName = "Guns/Bullet Penetration Config", order = 6)]
public class BulletPenetrationConfigScriptableObject : ScriptableObject, ICloneable
{
    /// <summary>
    /// 最大可穿透物体数量
    /// </summary>
    public int MaxObjectsToPenetrate = 0;
    
    /// <summary>
    /// 最大穿透深度
    /// </summary>
    public float MaxPenetrationDepth = 0.275f;
    
    /// <summary>
    /// 穿透后的精度损失值
    /// </summary>
    public Vector3 AccuracyLoss = new(0.1f, 0.1f, 0.1f);
    
    /// <summary>
    /// 穿透后伤害保持百分比
    /// </summary>
    public float DamageRetentionPercentage;
        
    /// <summary>
    /// 创建当前对象的副本
    /// </summary>
    /// <returns>返回一个新的BulletPenetrationConfigScriptableObject实例，包含与当前对象相同的属性值</returns>
    public object Clone()
    {
        // 创建新的配置对象实例
        BulletPenetrationConfigScriptableObject config = CreateInstance<BulletPenetrationConfigScriptableObject>();

        // 复制当前对象的属性值到新实例
        Utilities.CopyValues(this, config);

        return config;
    }
}
