using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人移动控制组件，负责控制敌人在NavMesh上的随机漫游行为。
/// 需要NavMeshAgent组件支持。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, ISlowable
{
    private Animator Animator;
    [SerializeField]
    private float StillDelay = 1f;
    private LookAtIK LookAt;
    private NavMeshAgent Agent;

    private Coroutine SlowCoroutine;
    private float BaseSpeed;
    private const string IsWalking = "IsWalking";
    
    private static NavMeshTriangulation Triangulation;
    
    // 性能优化相关
    private WaitForSeconds cachedWait;
    private Coroutine roamCoroutine;
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    private const float PositionCheckInterval = 0.5f;

    /// <summary>
    /// 初始化组件引用和基础数据。
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
        
        cachedWait = new WaitForSeconds(StillDelay);
        
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;
        
        BaseSpeed = Agent.speed;
    }

    /// <summary>
    /// 启动敌人漫游协程。
    /// </summary>
    private void Start()
    {
        StartCoroutine(Roam());
    }

    // private void OnEnable()
    // {
    //     if (roamCoroutine == null)
    //     {
    //         roamCoroutine = StartCoroutine(RoamOptimized());
    //     }
    // }
    //
    // private void OnDisable()
    // {
    //     if (roamCoroutine != null)
    //     {
    //         StopCoroutine(roamCoroutine);
    //         roamCoroutine = null;
    //     }
    // }

    /// <summary>
    /// 每帧更新动画状态和LookAt目标。
    /// </summary>
    private void Update()
    {
        // 优化动画更新 - 只在必要时更新
        UpdateAnimationState();
        
        // 优化LookAt更新
        UpdateLookAtTarget();
    }

    /// <summary>
    /// 根据位置变化更新动画状态。
    /// </summary>
    private void UpdateAnimationState()
    {
        // 每隔一段时间检查一次位置变化，避免每帧计算
        if (Time.time - lastPositionCheckTime > PositionCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            bool isMoving = distanceMoved > 0.01f;
            
            if (Animator != null)
            {
                Animator.SetBool(IsWalking, isMoving);
            }
            
            lastPosition = transform.position;
            lastPositionCheckTime = Time.time;
        }
    }

    /// <summary>
    /// 更新LookAt的目标位置。
    /// </summary>
    private void UpdateLookAtTarget()
    {
        if (LookAt != null && Agent != null && Agent.isOnNavMesh)
        {
            // 减少LookAt更新频率
            LookAt.lookAtTargetPosition = Agent.steeringTarget + transform.forward;
        }
    }

    /// <summary>
    /// 控制敌人在NavMesh上随机漫游的协程。
    /// </summary>
    /// <returns>IEnumerator用于协程控制</returns>
    private IEnumerator Roam()
    {
        // 添加随机延迟，避免所有敌人同时计算路径
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        
        WaitForSeconds pathUpdateDelay = new WaitForSeconds(Random.Range(1f, 3f));

        while (IsAgentValid())
        {
            Vector3 randomDestination = GetRandomNavMeshPoint();
            Agent.SetDestination(randomDestination);
            
            // 限制寻路尝试时间
            float pathTimeout = 3f;
            float pathStartTime = Time.time;
            
            while (Agent.pathPending && Time.time - pathStartTime < pathTimeout)
            {
                if (!IsAgentValid()) yield break;
                yield return null;
            }
            
            // 添加更多安全检查
            if (IsAgentValid() && Agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                // 等待到达目标点，添加安全检查
                float waitTime = 0f;
                const float maxWaitTime = 5f;
                
                while (waitTime < maxWaitTime)
                {
                    // 检查代理状态
                    if (!IsAgentValid()) break;
                        
                    // 检查是否已到达目标点
                    try
                    {
                        if (Agent.remainingDistance <= Agent.stoppingDistance)
                            break;
                    }
                    catch (System.Exception)
                    {
                        // 如果访问remainingDistance出错，则跳出循环
                        break;
                    }
                        
                    waitTime += Time.deltaTime;
                    yield return null;
                }
            }
            
            // 随机停留时间
            if (IsAgentValid() && cachedWait != null)
            {
                yield return cachedWait;
            }
            
            // 随机路径更新延迟
            if (IsAgentValid())
            {
                yield return pathUpdateDelay;
            }
        }
    }


    /// <summary>
    /// 检查NavMeshAgent是否有效。
    /// </summary>
    /// <returns>如果NavMeshAgent有效返回true，否则返回false</returns>
    private bool IsAgentValid()
    {
        return Agent != null && Agent.isOnNavMesh && Agent.isActiveAndEnabled && enabled;
    }

    /// <summary>
    /// 获取一个随机的NavMesh点。
    /// </summary>
    /// <returns>随机的NavMesh点坐标</returns>
    private Vector3 GetRandomNavMeshPoint()
    {
        // 使用更高效的随机点查找
        Vector3 randomDirection = Random.insideUnitSphere * 15f;
        randomDirection.y = 0f; // 保持在同一水平面上
        Vector3 randomPoint = transform.position + randomDirection;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 15f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    /// <summary>
    /// 停止敌人的移动。
    /// </summary>
    public void StopMoving()
    {
        // 停止所有协程
        StopAllCoroutines();
    
        // 安全地处理NavMeshAgent
        if (Agent != null)
        {
            Agent.isStopped = true;
            Agent.enabled = false;
        }
    }

    /// <summary>
    /// 应用减速效果。
    /// </summary>
    /// <param name="SlowCurve">描述减速过程的动画曲线</param>
    public void Slow(AnimationCurve SlowCurve)
    {
        if (SlowCoroutine != null)
        {
            StopCoroutine(SlowCoroutine);
        }
        SlowCoroutine = StartCoroutine(SlowDown(SlowCurve));
    }

    /// <summary>
    /// 执行减速效果的协程。
    /// </summary>
    /// <param name="SlowCurve">描述减速过程的动画曲线</param>
    /// <returns>IEnumerator用于协程控制</returns>
    private IEnumerator SlowDown(AnimationCurve SlowCurve)
    {
        float time = 0;

        while (time < SlowCurve.keys[^1].time)
        {
            Agent.speed = BaseSpeed * SlowCurve.Evaluate(time);
            time += Time.deltaTime;
            yield return null;
        }

        Agent.speed = BaseSpeed;
    }
}
