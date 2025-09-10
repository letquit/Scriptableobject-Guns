using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家行为控制类，用于处理玩家的射击输入操作
/// </summary>
[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;

    /// <summary>
    /// 每帧更新检查玩家输入状态，处理射击逻辑
    /// </summary>
    private void Update()
    {
        // 检查鼠标左键是否按下且当前有激活的武器
        if (Mouse.current.leftButton.isPressed
            && GunSelector.ActiveGun != null)
        {
            // 执行射击操作
            GunSelector.ActiveGun.Shoot();
        }
    }
}

