using UnityEngine;

/// <summary>
/// 碰撞处理接口，定义了处理碰撞事件的方法
/// </summary>
public interface ICollisionHandler
{
    /// <summary>
    /// 处理碰撞冲击事件
    /// </summary>
    /// <param name="ImpactedObject">被撞击的碰撞体对象</param>
    /// <param name="HitPosition">撞击发生的位置坐标</param>
    /// <param name="HitNormal">撞击点的法线向量</param>
    /// <param name="Gun">触发撞击的枪械配置对象</param>
    void HandleImpact(
        Collider ImpactedObject,
        Vector3 HitPosition,
        Vector3 HitNormal,
        GunScriptableObject Gun
    );
}

