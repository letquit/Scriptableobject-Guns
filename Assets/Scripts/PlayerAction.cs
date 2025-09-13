using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 玩家行为控制类，用于处理玩家的射击输入操作
/// </summary>
[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;
    [SerializeField] 
    private bool AutoReload = true;
    [SerializeField]
    private Animator PlayerAnimator;
    [SerializeField]
    private PlayerIK InverseKinematics;
    [SerializeField]
    private Image Crosshair;

    private bool IsReloading;

    /// <summary>
    /// 每帧更新检查玩家输入状态，处理射击逻辑
    /// </summary>
    private void Update()
    {
        // 当存在激活的武器且未在换弹状态并且应用处于焦点状态时，更新武器射击状态
        if (GunSelector.ActiveGun != null && !IsReloading && Application.isFocused)
        {
            GunSelector.ActiveGun.Tick(Mouse.current.leftButton.isPressed);
        }

        // 检查是否需要进行手动或自动换弹
        if (ShouldManualReload() || ShouldAutoReload())
        {
            GunSelector.ActiveGun.StartReloading();
            IsReloading = true;
            PlayerAnimator.SetTrigger("Reload");
            InverseKinematics.HandIKAmount = 0.25f;
            InverseKinematics.ElbowIKAmount = 0.25f;
        }
        
        UpdateCrosshair();
    }
    
    /// <summary>
    /// 更新准星位置，使其指向枪口射线与场景的交点
    /// </summary>
    private void UpdateCrosshair()
    {
        if (GunSelector.ActiveGun.ShootConfig.ShootType == ShootType.FromGun)
        {
            Vector3 gunTipPoint = GunSelector.ActiveGun.GetRaycastOrigin();
            Vector3 forward = GunSelector.ActiveGun.GetGunForward();

            Vector3 hitPoint = gunTipPoint + forward * 10;
            if (Physics.Raycast(gunTipPoint, forward, out RaycastHit hit, float.MaxValue, GunSelector.ActiveGun.ShootConfig.HitMask))
            {
                hitPoint = hit.point;
            }
            Vector3 screenSpaceLocation = GunSelector.Camera.WorldToScreenPoint(hitPoint);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)Crosshair.transform.parent,
                    screenSpaceLocation,
                    null,
                    out Vector2 localPosition))
            {
                Crosshair.rectTransform.anchoredPosition = localPosition;
            }
            else
            {
                Crosshair.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 结束换弹过程，恢复武器状态和IK权重
    /// </summary>
    private void EndReload()
    {
        GunSelector.ActiveGun.EndReload();
        InverseKinematics.HandIKAmount = 1f;
        InverseKinematics.ElbowIKAmount = 1f;
        IsReloading = false;
    }

    /// <summary>
    /// 判断是否需要手动换弹
    /// </summary>
    /// <returns>当按下R键且当前武器可以换弹时返回true，否则返回false</returns>
    private bool ShouldManualReload()
    {
        return !IsReloading
            && Keyboard.current.rKey.wasReleasedThisFrame
            && GunSelector.ActiveGun.CanReload();
    }
    
    /// <summary>
    /// 判断是否需要自动换弹
    /// </summary>
    /// <returns>当启用自动换弹且当前弹夹为空且武器可以换弹时返回true，否则返回false</returns>
    private bool ShouldAutoReload()
    {
        return !IsReloading
               && AutoReload
               && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0 
               && GunSelector.ActiveGun.CanReload();
    }
}
