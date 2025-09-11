using UnityEngine;

/// <summary>
/// 敌人类，负责管理敌人的整体行为和组件协调
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyHealth Health;
    public EnemyMovement Movement;
    public EnemyPainResponse PainResponse;

    /// <summary>
    /// 初始化敌人组件间的事件订阅关系
    /// 在游戏开始时将受伤和死亡事件与对应的处理函数进行绑定
    /// </summary>
    private void Start()
    {
        // 订阅受伤事件，当敌人受到伤害时触发疼痛反应
        Health.OnTakeDamage += PainResponse.HandlePain;
        // 订阅死亡事件，当敌人死亡时触发死亡处理函数
        Health.OnDeath += Die;
    }

    /// <summary>
    /// 处理敌人死亡逻辑
    /// 停止敌人的移动并触发死亡动画或效果
    /// </summary>
    /// <param name="Position">死亡时的位置信息</param>
    private void Die(Vector3 Position)
    {
        // 停止敌人移动
        Movement.StopMoving();
        // 处理死亡后的视觉效果和动画
        PainResponse.HandleDeath();
    }
}
