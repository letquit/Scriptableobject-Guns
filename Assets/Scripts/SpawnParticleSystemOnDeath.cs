using System;
using UnityEngine;

/// <summary>
/// 在对象死亡时生成粒子系统的组件
/// 该组件需要附加到实现了IDamageable接口的游戏对象上
/// 当对象死亡时，会在死亡位置生成指定的粒子系统
/// </summary>
[RequireComponent(typeof(IDamageable))]
public class SpawnParticleSystemOnDeath : MonoBehaviour
{
    [SerializeField] 
    private ParticleSystem DeathSystem;

    public IDamageable Damageable;

    /// <summary>
    /// 在Awake阶段获取组件引用
    /// </summary>
    private void Awake()
    {
        Damageable = GetComponent<IDamageable>();
    }

    /// <summary>
    /// 在组件启用时订阅死亡事件
    /// </summary>
    private void OnEnable()
    {
        Damageable.OnDeath += Damageable_OnDeath;
    }

    /// <summary>
    /// 处理死亡事件，在指定位置生成死亡粒子系统
    /// </summary>
    /// <param name="Position">死亡时生成粒子系统的世界坐标位置</param>
    private void Damageable_OnDeath(Vector3 Position)
    {
        // Instantiate(DeathSystem, Position, Quaternion.identity);
        
        // 实例化粒子系统
        ParticleSystem instance = Instantiate(DeathSystem, Position, Quaternion.identity);
        
        // 获取粒子系统组件的main模块以获取持续时间
        float duration = instance.main.duration;
        
        // 销毁粒子系统实例，延迟时间为持续时间加上一些缓冲时间
        Destroy(instance.gameObject, duration + 1f);
    }
}

