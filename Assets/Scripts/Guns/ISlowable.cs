using UnityEngine;

/// <summary>
/// 定义可被减速的对象接口
/// </summary>
public interface ISlowable
{
    /// <summary>
    /// 对对象应用减速效果
    /// </summary>
    /// <param name="SlowDecay">减速衰减曲线，用于控制减速效果随时间的变化</param>
    void Slow(AnimationCurve SlowDecay);
}

