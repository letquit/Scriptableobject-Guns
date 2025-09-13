using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 抽象值修改器类，用于修改指定属性的值
/// </summary>
/// <typeparam name="T">要修改的属性值的类型</typeparam>
public abstract class AbstractValueModifier<T> : IModifier
{
    public string AttributeName;
    public T Amount;
    public abstract void Apply(GunScriptableObject Gun);

    /// <summary>
    /// 获取指定路径的属性值
    /// </summary>
    /// <typeparam name="FieldType">属性字段的类型</typeparam>
    /// <param name="Gun">要从中获取属性值的枪械对象</param>
    /// <param name="TargetObject">输出参数，返回属性所在的对象实例</param>
    /// <param name="Field">输出参数，返回属性的字段信息</param>
    /// <returns>指定属性的当前值</returns>
    /// <exception cref="InvalidPathSpecifiedException">当指定的属性路径不存在时抛出异常</exception>
    protected FieldType GetAttribute<FieldType>(
        GunScriptableObject Gun,
        out object TargetObject,
        out FieldInfo Field)
    {
        // 解析属性路径，支持多级路径（用'/'分隔）
        string[] paths = AttributeName.Split('/');
        string attribute = paths[paths.Length - 1];
        
        Type type = Gun.GetType();
        object target = Gun;

        // 遍历路径中的每一级，逐层获取目标对象
        for (int i = 0; i < paths.Length - 1; i++)
        {
            FieldInfo field = type.GetField(paths[i]);
            if (field == null)
            {
                Debug.LogError($"Unable to apply modifier" +
                               $" to attribute {AttributeName} because it does not exist on gun {Gun}");
                throw new InvalidPathSpecifiedException(AttributeName);
            }
            else
            {
                target = field.GetValue(target);
                type = target.GetType();
            }
        }
        
        // 获取最终属性的字段信息
        FieldInfo attributeField = type.GetField(attribute);
        if (attributeField == null)
        {
            Debug.LogError($"Unable to apply modifier to attribute " +
                           $"{AttributeName} because it does not exist on gun {Gun}");
            throw new InvalidPathSpecifiedException(AttributeName);
        }
        
        Field = attributeField;
        TargetObject = target;
        return (FieldType)attributeField.GetValue(target);
    }
}
