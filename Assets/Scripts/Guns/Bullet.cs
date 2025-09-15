using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 子弹类，用于处理子弹的物理运动、碰撞检测和生命周期管理
/// 需要挂载到包含Rigidbody组件的游戏对象上
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private int ObjectsPenetrated;
    
    /// <summary>
    /// 获取当前子弹关联的刚体组件
    /// </summary>
    public Rigidbody Rigidbody { get; private set; }

    /// <summary>
    /// 获取或设置子弹生成时的位置
    /// </summary>
    [field: SerializeField] 
    public Vector3 SpawnLocation { get; private set; }

    /// <summary>
    /// 获取或设置子弹生成时的速度
    /// </summary>
    [field: SerializeField] 
    public Vector3 SpawnVelocity { get; private set; }

    /// <summary>
    /// 子弹在发射后自动销毁前的延迟时间（秒）
    /// </summary>
    [SerializeField]
    private float DelayedDisableTime = 2f;

    /// <summary>
    /// 子弹碰撞事件委托
    /// </summary>
    /// <param name="Bullet">发生碰撞的子弹实例</param>
    /// <param name="Collision">碰撞信息，如果为null表示超时销毁</param>
    /// <param name="ObjectsPenetrated">已穿透物体的数量</param>
    public delegate void CollisionEvent(Bullet Bullet, Collision Collision, int ObjectsPenetrated);
    
    /// <summary>
    /// 子弹碰撞事件，在子弹发生碰撞或超时销毁时触发
    /// </summary>
    public event CollisionEvent OnCollision;

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 发射子弹
    /// </summary>
    /// <param name="SpawnForce">子弹的初始发射力</param>
    public void Spawn(Vector3 SpawnForce)
    {
        ObjectsPenetrated = 0;
        SpawnLocation = transform.position;
        transform.forward = SpawnForce.normalized;
        Rigidbody.AddForce(SpawnForce);
        SpawnVelocity = SpawnForce * Time.fixedDeltaTime / Rigidbody.mass;
        StartCoroutine(DelayedDisable(DelayedDisableTime));
    }

    /// <summary>
    /// 延迟禁用协程，在指定时间后销毁子弹
    /// </summary>
    /// <param name="Time">延迟时间（秒）</param>
    /// <returns>协程迭代器</returns>
    private IEnumerator DelayedDisable(float Time)
    {
        yield return null;
        yield return new WaitForSeconds(Time);
        OnCollisionEnter(null);
    }
    
    /// <summary>
    /// 碰撞检测回调函数
    /// 当子弹与其它物体发生碰撞时调用，或在超时销毁时手动调用
    /// </summary>
    /// <param name="Collision">碰撞信息，若为null表示是超时销毁</param>
    private void OnCollisionEnter(Collision Collision)
    {
        OnCollision?.Invoke(this, Collision, ObjectsPenetrated);
        ObjectsPenetrated++;
    }

    /// <summary>
    /// 组件禁用时的清理工作
    /// 停止所有协程并重置物理状态及事件监听
    /// </summary>
    private void OnDisable()
    {
        StopAllCoroutines();
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        OnCollision = null;
    }
}
