using System;
using System.Reflection;

/// <summary>
/// 提供通用工具方法的静态类
/// </summary>
public class Utilities
{
    /// <summary>
    /// 将源对象的字段值复制到目标对象中
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="Base">源对象，从中复制字段值</param>
    /// <param name="Copy">目标对象，将字段值复制到此对象</param>
    public static void CopyValues<T>(T Base, T Copy)
    {
        // 获取对象的类型信息
        Type type = Base.GetType();
        
        // 遍历所有字段并复制值
        foreach (FieldInfo field in type.GetFields())
        {
            field.SetValue(Copy, field.GetValue(Base));
        }
    }
}

