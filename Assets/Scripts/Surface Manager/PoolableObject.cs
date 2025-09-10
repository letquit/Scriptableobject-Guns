using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 可池化的对象类，继承自MonoBehaviour
/// 用于管理对象池中的游戏对象，当对象被禁用时自动返回对象池
/// </summary>
public class PoolableObject : MonoBehaviour
{
    /// <summary>
    /// 父对象池引用，用于存储和管理当前游戏对象的生命周期
    /// </summary>
    public ObjectPool<GameObject> Parent;

    /// <summary>
    /// 当对象被禁用时调用此方法
    /// 如果存在父对象池，则将当前游戏对象释放回对象池中进行复用
    /// </summary>
    private void OnDisable()
    {
        // 检查父对象池是否存在，避免空引用异常
        if (Parent != null)
        {
            // 将当前游戏对象释放回对象池
            Parent.Release(gameObject);
        }
    }
}

