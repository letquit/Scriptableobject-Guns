using UnityEngine;

/// <summary>
/// 射击配置脚本化对象类
/// 用于存储和管理武器射击的相关配置参数
/// 可以通过Unity编辑器的菜单创建该资源文件
/// </summary>
[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigScriptableObject : ScriptableObject
{
    /// <summary>
    /// 子弹命中的层级掩码
    /// 用于确定子弹可以与哪些层级的物体发生碰撞检测
    /// </summary>
    public LayerMask HitMask;
    
    /// <summary>
    /// 射击散布向量
    /// 控制子弹发射时的随机散布范围，xyz分量分别对应不同方向的散布程度
    /// 默认值为(0.1, 0.1, 0.1)
    /// </summary>
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    
    /// <summary>
    /// 射击频率
    /// 控制连续射击时每发子弹之间的间隔时间(秒)
    /// 值越小射速越快，默认值0.25秒
    /// </summary>
    public float FireRate = 0.25f;
}

