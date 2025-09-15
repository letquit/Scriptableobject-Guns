    using UnityEngine;

    /// <summary>
    /// 子弹拾取物品类，当玩家碰撞时会为对应类型的武器添加弹药
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AmmoPickup : MonoBehaviour
    {
        public GunType Type;
        public int AmmoAmount = 30;
        public Vector3 SpinDirection = Vector3.up;

        /// <summary>
        /// 每帧更新拾取物品的旋转效果
        /// </summary>
        private void Update()
        {
            // 旋转拾取物品
            transform.Rotate(SpinDirection);
        }
        
        /// <summary>
        /// 当触发器被碰撞时调用，检测是否为玩家并为其武器添加弹药
        /// </summary>
        /// <param name="other">碰撞的碰撞器组件</param>
        private void OnTriggerEnter(Collider other)
        {
            // 检查碰撞对象是否为玩家武器选择器且当前激活武器类型匹配
            if (other.TryGetComponent(out PlayerGunSelector gunSelector) && gunSelector.ActiveGun.Type == Type)
            {
                // 为武器添加弹药并销毁拾取物品
                gunSelector.ActiveGun.AmmoConfig.AddAmmo(AmmoAmount);
                Destroy(gameObject);
            }
        }
    }

