using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// 伤害配置脚本化对象类，用于定义武器在不同距离下的伤害曲线配置
/// </summary>
[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfigScriptableObject : ScriptableObject
{
    /// <summary>
    /// 伤害曲线，定义了距离与伤害值的关系
    /// </summary>
    public MinMaxCurve DamageCurve;

    /// <summary>
    /// 重置函数，在脚本首次添加到对象时调用，初始化伤害曲线模式为曲线模式
    /// </summary>
    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    /// <summary>
    /// 根据距离获取伤害值
    /// </summary>
    /// <param name="Distance">距离参数，用于计算在该距离下的伤害值</param>
    /// <returns>计算得到的伤害值，返回整数类型</returns>
    public int GetDamage(float Distance = 0)
    {
        // 使用伤害曲线评估指定距离下的伤害值，并向上取整为整数
        return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));
    }
}
