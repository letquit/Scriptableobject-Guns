using System;
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

    [SerializeField] private GunScriptableObject ActiveBaseGun;


    /// <summary>
    /// 在对象被唤醒时调用，初始化当前枪械配置
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
        
        SetupGun(gun);
    }

    /// <summary>
    /// 设置指定枪械为当前使用枪械，并生成其实例
    /// </summary>
    /// <param name="Gun">要设置的枪械ScriptableObject配置</param>
    private void SetupGun(GunScriptableObject Gun)
    {
        ActiveBaseGun = Gun;
        // 克隆枪械配置对象并激活使用
        ActiveGun = Gun.Clone() as GunScriptableObject;
        // 如果克隆成功，则在指定父对象下生成枪械实例
        ActiveGun?.Spawn(GunParent, this, Camera);

        InverseKinematics.SetGunStyle(ActiveGun.Type == GunType.Glock);
        InverseKinematics.Setup(GunParent);
    }

    /// <summary>
    /// 销毁当前激活的枪械实例
    /// </summary>
    public void DespawnActiveGun()
    {
        ActiveGun.Despawn();
        Destroy(ActiveGun);
    }
    
    /// <summary>
    /// 拾取新的枪械并替换当前枪械
    /// </summary>
    /// <param name="Gun">要拾取的新枪械ScriptableObject配置</param>
    public void PickupGun(GunScriptableObject Gun)
    {
        DespawnActiveGun();
        SetupGun(Gun);
    }
    
    /// <summary>
    /// 应用一组修饰符到当前枪械上，会重新生成枪械实例并应用所有修饰符效果
    /// </summary>
    /// <param name="Modifiers">要应用的修饰符数组</param>
    public void ApplyModifiers(IModifier[] Modifiers)
    {
        DespawnActiveGun();
        SetupGun(ActiveBaseGun);

        foreach (IModifier modifier in Modifiers)
        {
            modifier.Apply(ActiveGun);
        }
    }
}
