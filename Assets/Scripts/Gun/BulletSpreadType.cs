/// <summary>
/// 子弹散布类型枚举，定义了不同类型的子弹散布计算方式
/// </summary>
public enum BulletSpreadType
{
    /// <summary>
    /// 节点型散布 - 基于预定义的节点位置进行散布计算
    /// </summary>
    Node,
    
    /// <summary>
    /// 简单型散布 - 使用基础的随机算法进行散布计算
    /// </summary>
    Simple,
    
    /// <summary>
    /// 基于纹理的散布 - 根据纹理数据来决定子弹的散布模式
    /// </summary>
    TextureBased
}
