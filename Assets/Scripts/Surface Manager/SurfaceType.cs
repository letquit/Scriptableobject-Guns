using UnityEngine;

/// <summary>
/// SurfaceType类用于定义表面类型的材质属性
/// 该类可序列化，能够在Unity编辑器中显示和编辑其属性
/// </summary>
[System.Serializable]
public class SurfaceType
{
    /// <summary>
    /// 表面的反照率贴图（Albedo Map）
    /// 用于定义表面的基础颜色和纹理信息
    /// </summary>
    public Texture Albedo;
    
    /// <summary>
    /// 表面属性配置对象
    /// 包含表面的物理特性和渲染参数
    /// </summary>
    public Surface Surface;
}
