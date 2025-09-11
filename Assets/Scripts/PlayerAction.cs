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
        // 如果当前有激活的武器，则更新武器状态
        if (GunSelector.ActiveGun != null)
        {
            GunSelector.ActiveGun.Tick(Mouse.current.leftButton.isPressed);
        }
    }


}

