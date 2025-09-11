using UnityEngine;
using UnityEngine.AI;

// Sourced from: https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
// slightly modified
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class LookAtIK : MonoBehaviour
{
    /// <summary>
    /// 头部的Transform引用，用于IK注视目标计算。
    /// </summary>
    public Transform head = null;

    /// <summary>
    /// 当前注视的目标位置。
    /// </summary>
    public Vector3 lookAtTargetPosition;

    /// <summary>
    /// 注视权重从0到1的冷却时间（秒），控制IK注视效果减弱的速度。
    /// </summary>
    public float lookAtCoolTime = 0.2f;

    /// <summary>
    /// 注视权重从0到1的加热时间（秒），控制IK注视效果增强的速度。
    /// </summary>
    public float lookAtHeatTime = 0.2f;

    /// <summary>
    /// 是否启用注视IK功能。
    /// </summary>
    public bool looking = true;

    private Vector3 lookAtPosition;
    private NavMeshAgent Agent;
    private Animator anim;
    private float lookAtWeight = 0.0f;

    /// <summary>
    /// 初始化获取NavMeshAgent组件。
    /// </summary>
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 在Start中初始化Animator组件，并设置初始注视目标位置。
    /// 如果没有指定head，则禁用该脚本并输出错误日志。
    /// </summary>
    void Start()
    {
        if (!head)
        {
            Debug.LogError("No head transform - LookAt disabled");
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
        lookAtTargetPosition = head.position + transform.forward;
        lookAtPosition = lookAtTargetPosition;
    }

    /// <summary>
    /// 每帧更新当前的注视目标位置为导航代理的转向目标加上角色前方偏移。
    /// </summary>
    private void Update()
    {
        lookAtTargetPosition = Agent.steeringTarget + transform.forward;
    }

    /// <summary>
    /// Unity动画IK回调函数，在这里处理头部朝向目标的IK逻辑。
    /// 包括平滑插值当前注视方向、动态调整注视权重以及设置Animator中的LookAt参数。
    /// </summary>
    void OnAnimatorIK()
    {
        // 保持Y轴一致以避免头部倾斜
        lookAtTargetPosition.y = head.position.y;

        // 根据是否启用looking决定目标权重
        float lookAtTargetWeight = looking ? 1.0f : 0.0f;

        // 计算当前与未来注视方向向量
        Vector3 curDir = lookAtPosition - head.position;
        Vector3 futDir = lookAtTargetPosition - head.position;

        // 平滑旋转当前方向朝向目标方向，最大角速度为每秒一圈（6.28弧度）
        curDir = Vector3.RotateTowards(curDir, futDir, 6.28f * Time.deltaTime, float.PositiveInfinity);
        lookAtPosition = head.position + curDir;

        // 根据目标权重变化选择加热或冷却时间进行插值
        float blendTime = lookAtTargetWeight > lookAtWeight ? lookAtHeatTime : lookAtCoolTime;
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtTargetWeight, Time.deltaTime / blendTime);

        // 设置Animator的LookAt参数
        anim.SetLookAtWeight(lookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
        anim.SetLookAtPosition(lookAtPosition);
    }
}
