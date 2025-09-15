using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 枪械拾取物品类，用于在游戏中创建可拾取的枪械对象
/// 该类需要配合Collider组件使用，当玩家碰撞时会自动拾取对应的枪械
/// </summary>
[RequireComponent(typeof(Collider))]
public class GunPickup : MonoBehaviour
{
   public GunScriptableObject Gun;
   public Vector3 SpinDirection = Vector3.up;

   /// <summary>
   /// 每帧更新方法，用于旋转枪械模型
   /// 使枪械在场景中以指定方向持续旋转展示
   /// </summary>
   private void Update()
   {
      transform.Rotate(SpinDirection);
   }

   /// <summary>
   /// 触发器进入事件处理方法
   /// 当其他碰撞体进入触发器时调用，用于处理玩家拾取枪械的逻辑
   /// </summary>
   /// <param name="other">进入触发器的碰撞体组件</param>
   private void OnTriggerEnter(Collider other)
   {
      /// 检查碰撞体是否属于玩家的枪械选择器组件，如果是则执行拾取逻辑
      if (other.TryGetComponent(out PlayerGunSelector gunSelector))
      {
         gunSelector.PickupGun(Gun);
         Destroy(gameObject);
      }
   }
}
