using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Surface类用于定义不同表面类型的碰撞效果配置
/// 该类继承自ScriptableObject，可以在Unity编辑器中创建资产文件
/// 通过CreateAssetMenu属性，用户可以在编辑器中通过菜单创建Surface资产
/// </summary>
[CreateAssetMenu(menuName = "Impact System/Surface", fileName = "Surface")]
public class Surface : ScriptableObject
{
    /// <summary>
    /// SurfaceImpactTypeEffect类用于存储碰撞类型与表面效果的映射关系
    /// 该类可序列化，以便在Unity编辑器中进行配置
    /// </summary>
    [Serializable]
    public class SurfaceImpactTypeEffect
    {
        /// <summary>
        /// 碰撞类型引用，定义了碰撞的种类
        /// </summary>
        public ImpactType ImpactType;

        /// <summary>
        /// 表面效果引用，定义了在该表面类型上产生碰撞时的效果
        /// </summary>
        public SurfaceEffect SurfaceEffect;
    }

    /// <summary>
    /// 存储碰撞类型与表面效果映射关系的列表
    /// 每个元素定义了一种碰撞类型在该表面产生的特定效果
    /// </summary>
    public List<SurfaceImpactTypeEffect> ImpactTypeEffects = new List<SurfaceImpactTypeEffect>();
}
