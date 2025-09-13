using System;
using UnityEngine;

/// <summary>
/// 自定义异常类，用于表示路径指定无效的错误情况
/// </summary>
public class InvalidPathSpecifiedException : Exception
{
    /// <summary>
    /// 初始化 InvalidPathSpecifiedException 类的新实例
    /// </summary>
    /// <param name="AttributeName">引发异常的属性名称</param>
    public InvalidPathSpecifiedException(string AttributeName): base($"{AttributeName} does not exist at the provided path!") { }
}

