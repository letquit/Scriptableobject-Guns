/// <summary>
/// ImpactTypeModifier类用于修改枪械的冲击类型属性
/// 继承自AbstractValueModifier<ImpactType>泛型抽象类
/// </summary>
public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
{
    /// <summary>
    /// 应用冲击类型修改器到指定的枪械对象
    /// 将枪械的ImpactType属性设置为修改器的Amount值
    /// </summary>
    /// <param name="Gun">要应用修改器的枪械脚本对象</param>
    public override void Apply(GunScriptableObject Gun)
    {
        // 将修改器的Amount值赋给枪械的ImpactType属性
        Gun.ImpactType = Amount;
    }
}
