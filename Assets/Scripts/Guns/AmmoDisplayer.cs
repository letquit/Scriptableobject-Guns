using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 弹药显示器类，用于在UI上显示当前武器的弹药数量
/// 该组件会自动获取TextMeshProUGUI组件并定期更新显示内容
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
public class AmmoDisplayer : MonoBehaviour
{
    [SerializeField] 
    private PlayerGunSelector GunSelector;
    private TextMeshProUGUI AmmoText;

    /// <summary>
    /// 组件初始化方法，在Awake阶段获取TextMeshProUGUI组件引用
    /// </summary>
    private void Awake()
    {
        AmmoText = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 每帧更新弹药显示文本
    /// 格式为"当前弹夹子弹数 / 总备弹数"
    /// </summary>
    private void Update()
    {
        // 构造并设置弹药显示文本
        AmmoText.SetText(
            $"{GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo} / " +
            $"{GunSelector.ActiveGun.AmmoConfig.CurrentAmmo}"
        );
    }
}

