using UnityEngine;

/// <summary>
/// 武器修饰器接口，用于定义可以应用到枪械上的修饰器
/// </summary>
public interface IModifier
{
    /// <summary>
    /// 应用修饰器到指定的枪械对象上
    /// </summary>
    /// <param name="Gun">要应用修饰器的枪械脚本化对象</param>
    void Apply(GunScriptableObject Gun);
}

