using UnityEngine;

/// <summary>
/// 死亡销毁回调类
/// 当对象死亡时执行销毁操作
/// </summary>
public class DeathDestroyCallback : MonoBehaviour
{
    /// <summary>
    /// 死亡结束回调函数
    /// 在死亡动画或死亡过程结束后调用，用于销毁当前游戏对象
    /// </summary>
    public void DeathEnd()
    {
        // 销毁当前游戏对象
        Destroy(gameObject);
    }
}
