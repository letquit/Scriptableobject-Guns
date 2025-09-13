using System.Reflection;
using UnityEngine;

/// <summary>
/// Vector3Modifier类用于修改Vector3类型的属性值
/// 该类继承自AbstractValueModifier<Vector3>，专门处理Vector3类型的数值修改
/// </summary>
public class Vector3Modifier : AbstractValueModifier<Vector3>
{
    /// <summary>
    /// 应用修改器到指定的枪械对象上
    /// 该方法会根据Amount属性的值来缩放目标Vector3属性的各个分量
    /// </summary>
    /// <param name="Gun">要应用修改器的枪械脚本对象</param>
    public override void Apply(GunScriptableObject Gun)
    {
        try
        {
            // 获取目标对象的Vector3属性值，并通过Amount属性进行缩放修改
            Vector3 value = GetAttribute<Vector3>(Gun, out object targetObject, out FieldInfo field);
            value = new(value.x * Amount.x, value.y * Amount.y, value.z * Amount.z);
            field.SetValue(targetObject, value);
        }
        catch (InvalidPathSpecifiedException) { }
    }
}

