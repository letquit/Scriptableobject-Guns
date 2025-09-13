using System;
using UnityEngine;


/// <summary>
/// 弹药配置脚本化对象类，用于管理武器的弹药系统
/// </summary>
[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
public class AmmoConfigScriptableObject : ScriptableObject, ICloneable
{
    /// <summary>
    /// 最大备用弹药数量
    /// </summary>
    public int MaxAmmo = 120;

    /// <summary>
    /// 弹夹容量
    /// </summary>
    public int ClipSize = 30;
    
    /// <summary>
    /// 当前备用弹药数量
    /// </summary>
    public int CurrentAmmo = 120;

    /// <summary>
    /// 当前弹夹中的子弹数量
    /// </summary>
    public int CurrentClipAmmo = 30;

    /// <summary>
    /// 使用弹药节省算法重新装弹。
    /// 这意味着它只会从 CurrentAmmo 中减去 ClipSize 和 CurrentClipAmmo 之间的增量。
    /// </summary>
    public void Reload()
    {
        // 计算最大可装填数量：弹夹容量和当前备用弹药的较小值
        int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
        // 计算当前弹夹还可装填的子弹数量
        int availableBulletsInCurrentClip = ClipSize - CurrentClipAmmo;
        // 确定实际装填数量：最大可装填数量和弹夹剩余容量的较小值
        int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);
        
        CurrentClipAmmo += reloadAmount;
        CurrentAmmo -= reloadAmount;
    }

    // /// <summary>
    // /// 装弹不节省弹药。
    // /// 这意味着它将始终从 CurrentAmmo（如果可用）中减去 ClipSize。
    // /// </summary>
    // public void Reload()
    // {
    //     int reloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
    //     CurrentClipAmmo = reloadAmount;
    //     CurrentAmmo -= reloadAmount;
    // }

    /// <summary>
    /// 检查是否可以进行装填操作
    /// </summary>
    /// <returns>当弹夹未满且还有备用弹药时返回true，否则返回false</returns>
    public bool CanReload()
    {
        return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
    }

    /// <summary>
    /// 创建当前弹药配置脚本化对象的克隆副本
    /// </summary>
    /// <returns>返回一个新的弹药配置脚本化对象实例，包含与当前对象相同的属性值</returns>
    public object Clone()
    {
        // 创建新的弹药配置脚本化对象实例
        AmmoConfigScriptableObject config = CreateInstance<AmmoConfigScriptableObject>();
        
        // 复制当前对象的所有属性值到新创建的实例中
        Utilities.CopyValues(this, config);
        
        return config;
    }

}

