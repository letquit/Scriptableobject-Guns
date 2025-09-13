using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 玩家枪械选择器组件，用于管理玩家角色的枪械装备和IK设置
/// </summary>
[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    /// <summary>
    /// 公共的Camera组件引用变量
    /// </summary>
    public Camera Camera;

    /// <summary>
    /// 枪械类型序列化字段，用于在Inspector中指定枪械类型
    /// </summary>
    [SerializeField]
    private GunType Gun;
    
    /// <summary>
    /// 枪械父级变换组件序列化字段，用于在Inspector中指定枪械的父级Transform
    /// </summary>
    [SerializeField]
    private Transform GunParent;
    
    /// <summary>
    /// 枪械ScriptableObject列表序列化字段，用于在Inspector中指定可用的枪械配置
    /// </summary>
    [SerializeField]
    private List<GunScriptableObject> Guns;
    
    /// <summary>
    /// 玩家IK组件序列化字段，用于在Inspector中指定玩家的逆向运动学组件
    /// </summary>
    [SerializeField]
    private PlayerIK InverseKinematics;

    /// <summary>
    /// 运行时填充区域标记
    /// </summary>
    [Space]
    [Header("Runtime Filled")]
    
    /// <summary>
    /// 当前激活的枪械ScriptableObject，运行时动态赋值
    /// </summary>
    public GunScriptableObject ActiveGun;


    /// <summary>
    /// 在游戏开始时初始化枪械和相关的IK目标设置
    /// </summary>
    private void Start()
    {
        // 查找并实例化指定类型的枪械
        GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

        if (gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
            return;
        }

        ActiveGun = gun;
        gun.Spawn(GunParent, this, Camera);

        // 设置IK目标点，用于动画反向动力学
        Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
        InverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        InverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
}

