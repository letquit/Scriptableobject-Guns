using System;
using UnityEngine;

/// <summary>
/// 子弹轨迹配置脚本化对象类
/// 用于定义子弹轨迹效果的各种参数配置，可以通过Unity编辑器创建和配置
/// </summary>
[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Trail Config", order = 4)]
public class TrailConfigScriptableObject : ScriptableObject, ICloneable
{
    /// <summary>
    /// 轨迹渲染使用的材质
    /// </summary>
    public Material Material;
    
    /// <summary>
    /// 轨迹宽度变化曲线
    /// 用于控制轨迹在生命周期内宽度的变化
    /// </summary>
    public AnimationCurve WidthCurve;
    
    /// <summary>
    /// 轨迹持续时间（秒）
    /// 轨迹从出现到消失的总时间
    /// </summary>
    public float Duration = 0.5f;
    
    /// <summary>
    /// 轨迹顶点间的最小距离
    /// 控制轨迹平滑度，值越小轨迹越平滑但性能消耗越大
    /// </summary>
    public float MinVertexDistance = 0.1f;
    
    /// <summary>
    /// 轨迹颜色渐变
    /// 定义轨迹在其生命周期内的颜色变化
    /// </summary>
    public Gradient Color;

    /// <summary>
    /// 子弹未命中目标时的飞行距离
    /// 当子弹未击中任何目标时，轨迹延伸的最大距离
    /// </summary>
    public float MissDistance = 100f;
    
    /// <summary>
    /// 轨迹模拟速度
    /// 控制轨迹点的生成速度和动画播放速度
    /// </summary>
    public float SimulationSpeed = 100f;

    /// <summary>
    /// 创建当前TrailConfigScriptableObject实例的副本
    /// </summary>
    /// <returns>返回一个新的TrailConfigScriptableObject对象，其属性值与当前实例相同</returns>
    public object Clone()
    {
        // 创建新的TrailConfigScriptableObject实例
        TrailConfigScriptableObject config = CreateInstance<TrailConfigScriptableObject>();
        
        // 复制当前实例的属性值到新实例
        Utilities.CopyValues(this, config);
        
        return config;
    }

}

