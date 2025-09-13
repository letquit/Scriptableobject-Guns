using UnityEngine;
using UnityEngine.AI;

// Sourced from: https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
// slightly modified
// 优化 LookAtIK.cs
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class LookAtIK : MonoBehaviour
{
    /// <summary>
    /// 头部的 Transform 组件，用于设置注视目标的位置参考。
    /// </summary>
    public Transform head = null;

    /// <summary>
    /// 当前角色希望注视的目标位置。
    /// </summary>
    public Vector3 lookAtTargetPosition;

    /// <summary>
    /// 注视权重从0到1变化所需的时间（冷却时间），用于平滑取消注视效果。
    /// </summary>
    public float lookAtCoolTime = 0.2f;

    /// <summary>
    /// 注视权重从0到1变化所需的时间（加热时间），用于平滑启用注视效果。
    /// </summary>
    public float lookAtHeatTime = 0.2f;

    /// <summary>
    /// 是否启用注视 IK 功能。
    /// </summary>
    public bool looking = true;

    /// <summary>
    /// 当前实际使用的注视位置，会逐渐向 lookAtTargetPosition 靠近。
    /// </summary>
    private Vector3 lookAtPosition;

    /// <summary>
    /// 导航代理组件，用于获取当前的移动方向和目标点。
    /// </summary>
    private NavMeshAgent Agent;

    /// <summary>
    /// 动画控制器组件，用于调用 SetLookAtWeight 和 SetLookAtPosition 方法。
    /// </summary>
    private Animator anim;

    /// <summary>
    /// 当前的注视权重，范围在 0 到 1 之间。
    /// </summary>
    private float lookAtWeight = 0.0f;
    
    /// <summary>
    /// 上一次更新时的角度差值，用于性能优化判断是否需要更新 IK。
    /// </summary>
    private float lastUpdateAngle = 0f;

    /// <summary>
    /// 角度变化阈值，只有当角度变化超过该值时才会更新 IK。
    /// </summary>
    private const float AngleThreshold = 5f; // 角度阈值

    /// <summary>
    /// 在 Awake 阶段获取 NavMeshAgent 组件。
    /// </summary>
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 初始化组件，在 Start 中检查 head 是否存在并初始化相关变量。
    /// 如果 head 不存在则禁用脚本。
    /// </summary>
    void Start()
    {
        if (!head)
        {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
        lookAtTargetPosition = head.position + transform.forward;
        lookAtPosition = lookAtTargetPosition;
    }

    /// <summary>
    /// 每帧更新 lookAtTargetPosition，使其跟随导航代理的转向目标。
    /// </summary>
    private void Update()
    {
        if (Agent != null && Agent.isOnNavMesh)
        {
            lookAtTargetPosition = Agent.steeringTarget + transform.forward;
        }
    }

    /// <summary>
    /// 在 Animator IK 回调中设置角色头部朝向目标位置。
    /// 包含注视权重的平滑过渡、角度变化检测以提升性能。
    /// </summary>
    void OnAnimatorIK()
    {
        if (head == null || anim == null) return;

        // 保持Y轴一致以避免头部倾斜
        lookAtTargetPosition.y = head.position.y;

        // 计算角度变化，只在变化较大时更新
        Vector3 currentDir = lookAtPosition - head.position;
        Vector3 targetDir = lookAtTargetPosition - head.position;
        float angleDiff = Vector3.Angle(currentDir, targetDir);
        
        // 如果角度变化小于阈值，并且当前注视权重较低，则跳过更新
        if (angleDiff < AngleThreshold && lookAtWeight < 0.1f)
        {
            return;
        }

        float lookAtTargetWeight = looking ? 1.0f : 0.0f;
        Vector3 curDir = Vector3.RotateTowards(currentDir, targetDir, 6.28f * Time.deltaTime, float.PositiveInfinity);
        lookAtPosition = head.position + curDir;

        float blendTime = lookAtTargetWeight > lookAtWeight ? lookAtHeatTime : lookAtCoolTime;
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtTargetWeight, Time.deltaTime / blendTime);

        if (lookAtWeight > 0.01f)
        {
            anim.SetLookAtWeight(lookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
            anim.SetLookAtPosition(lookAtPosition);
        }
    }
}
