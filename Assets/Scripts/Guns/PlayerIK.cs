using System.Linq;
using UnityEngine;

/// <summary>
/// PlayerIK 类用于控制角色的 IK（Inverse Kinematics）设置，允许手动控制手部和手肘的位置与旋转。
/// 此组件依赖 Animator 组件，并禁止挂载多个实例。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    /// <summary>
    /// 左手 IK 目标位置的 Transform 引用。
    /// </summary>
    public Transform LeftHandIKTarget;

    /// <summary>
    /// 右手 IK 目标位置的 Transform 引用。
    /// </summary>
    public Transform RightHandIKTarget;

    /// <summary>
    /// 左手肘 IK 目标位置的 Transform 引用。
    /// </summary>
    public Transform LeftElbowIKTarget;

    /// <summary>
    /// 右手肘 IK 目标位置的 Transform 引用。
    /// </summary>
    public Transform RightElbowIKTarget;

    /// <summary>
    /// 手部 IK 权重，取值范围为 0 到 1。
    /// </summary>
    [Range(0, 1f)]
    public float HandIKAmount = 1f;

    /// <summary>
    /// 手肘 IK 权重，取值范围为 0 到 1。
    /// </summary>
    [Range(0, 1f)]
    public float ElbowIKAmount = 1f;

    /// <summary>
    /// Animator 组件的引用。
    /// </summary>
    private Animator Animator;

    /// <summary>
    /// 在 Awake 阶段获取 Animator 组件。
    /// </summary>
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 在 Animator IK 阶段调用，用于设置手部和手肘的 IK 位置与旋转。
    /// </summary>
    /// <param name="layerIndex">当前处理的 Animator 层索引。</param>
    private void OnAnimatorIK(int layerIndex)
    {
        // 设置左手 IK 位置与旋转
        if (LeftHandIKTarget != null)
        {
            Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        }

        // 设置右手 IK 位置与旋转
        if (RightHandIKTarget != null)
        {
            Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmount);
            Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmount);
            Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
            Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
        }

        // 设置左手肘 IK 位置
        if (LeftElbowIKTarget != null)
        {
            Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
            Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmount);
        }

        // 设置右手肘 IK 位置
        if (RightElbowIKTarget != null)
        {
            Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
            Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmount);
        }
    }

    /// <summary>
    /// 设置枪械持握风格，切换是否为单手或双手持枪状态。
    /// </summary>
    /// <param name="OnHanded">若为 true，则启用单手持枪；否则启用双手持枪。</param>
    public void SetGunStyle(bool OnHanded)
    {
        Animator.SetBool("Is2HandedGun", !OnHanded);
        Animator.SetBool("Is1HandedGun", OnHanded);
    }
    
    /// <summary>
    /// 根据指定的武器父对象自动查找并设置 IK 目标点。
    /// </summary>
    /// <param name="GunParent">武器模型的根 Transform 对象。</param>
    public void Setup(Transform GunParent)
    {
        // 设置IK目标点，用于动画反向动力学
        Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
        LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
}
