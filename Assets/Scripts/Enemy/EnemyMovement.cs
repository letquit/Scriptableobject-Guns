using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敌人移动控制组件，负责控制敌人在NavMesh上的随机漫游行为
/// 需要NavMeshAgent组件支持
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private Animator Animator;
    [SerializeField]
    private float StillDelay = 1f;
    private LookAtIK LookAt;
    private NavMeshAgent Agent;

    private const string IsWalking = "IsWalking";

    private static NavMeshTriangulation Triangulation;

    /// <summary>
    /// 组件初始化，在Awake阶段获取必要的组件引用并初始化NavMesh三角剖分数据
    /// </summary>
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        LookAt = GetComponent<LookAtIK>();
        if (Triangulation.vertices == null || Triangulation.vertices.Length == 0)
        {
            Triangulation = NavMesh.CalculateTriangulation();
        }
    }

    /// <summary>
    /// 开始漫游，在Start阶段启动Roam协程
    /// </summary>
    private void Start()
    {
        StartCoroutine(Roam());
    }

    /// <summary>
    /// 每帧更新，同步动画状态和视线目标位置
    /// </summary>
    private void Update()
    {
        // 根据代理速度控制行走动画状态
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        if (LookAt != null)
        {
            // 设置视线目标为导航代理的转向目标点
            LookAt.lookAtTargetPosition = Agent.steeringTarget + transform.forward;
        }
    }

    /// <summary>
    /// 敌人随机漫游协程，在NavMesh上随机选择目标点进行移动
    /// </summary>
    /// <returns>IEnumerator迭代器对象</returns>
    private IEnumerator Roam()
    {
        WaitForSeconds wait = new WaitForSeconds(StillDelay);

        while (enabled)
        {
            // 随机选择三角剖分中的两个顶点之间的一个点作为目标位置
            int index = Random.Range(1, Triangulation.vertices.Length);
            Agent.SetDestination(
                Vector3.Lerp(
                    Triangulation.vertices[index - 1],
                    Triangulation.vertices[index],
                    Random.value
                )
            );
            // 等待到达目标点
            yield return new WaitUntil(() => Agent.remainingDistance <= Agent.stoppingDistance);
            // 停留指定时间
            yield return wait;
        }
    }

    /// <summary>
    /// 停止敌人的移动行为，停止所有协程并禁用NavMeshAgent
    /// </summary>
    public void StopMoving()
    {
        StopAllCoroutines();
        Agent.isStopped = true;
        Agent.enabled = false;
    }
}
