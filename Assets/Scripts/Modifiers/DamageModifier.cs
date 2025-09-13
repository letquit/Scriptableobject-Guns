using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 伤害修饰器类，用于修改武器的伤害值
/// </summary>
public class DamageModifier : AbstractValueModifier<float>
{
    /// <summary>
    /// 应用伤害修饰器到指定的武器对象
    /// </summary>
    /// <param name="Gun">要应用修饰器的武器脚本对象</param>
    public override void Apply(GunScriptableObject Gun)
    {
        try
        {
            // 获取武器对象中的粒子系统最小最大曲线属性
            ParticleSystem.MinMaxCurve damageCurve = GetAttribute<ParticleSystem.MinMaxCurve>(
                Gun,
                out object targetObject,
                out FieldInfo field);
            
            // 根据曲线模式应用不同的伤害倍率修改
            switch (damageCurve.mode)
            {
                case ParticleSystemCurveMode.TwoConstants:
                    damageCurve.constantMin *= Amount;
                    damageCurve.constantMax *= Amount;
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    damageCurve.curveMultiplier *= Amount;
                    break;
                case ParticleSystemCurveMode.Curve:
                    damageCurve.curveMultiplier *= Amount;
                    break;
                case ParticleSystemCurveMode.Constant:
                    damageCurve.constant *= Amount;
                    break;
            }
            
            // 将修改后的曲线值写回原对象
            field.SetValue(targetObject, damageCurve);
        }
        catch (InvalidPathSpecifiedException) { }
    }
}

