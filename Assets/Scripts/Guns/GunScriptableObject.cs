using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 枪械数据的 ScriptableObject 类，用于定义枪械的基本属性、射击配置和弹道特效。
/// </summary>
[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject, ICloneable
{
    public ImpactType ImpactType;
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public DamageConfigScriptableObject DamageConfig;
    public AmmoConfigScriptableObject AmmoConfig;
    public ShootConfigScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AudioConfigScriptableObject AudioConfig;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private Camera ActiveCamera;
    private AudioSource ShootingAudioSource;
    private float LastShootTime;
    private float InitialClickTime;
    private float StopShootingTIme;
    private bool LastFrameWantedToShoot;
    
    private ParticleSystem ShootSystem;
    private ObjectPool<Bullet> BulletPool;
    private ObjectPool<TrailRenderer> TrailPool;

    /// <summary>
    /// 初始化枪械模型并绑定到指定父对象上。
    /// </summary>
    /// <param name="Parent">枪械模型的父级 Transform。</param>
    /// <param name="ActiveMonoBehaviour">当前激活的 MonoBehaviour 实例，用于协程启动。</param>
    /// <param name="ActiveCamera">当前激活的摄像机实例，用于从摄像机位置进行射击。</param>
    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour, Camera ActiveCamera = null)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        this.ActiveCamera = ActiveCamera;
        
        // 创建子弹轨迹渲染器的对象池
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        if (!ShootConfig.IsHitscan)
        {
            BulletPool = new ObjectPool<Bullet>(CreateBullet);
        }
        
        // 实例化枪械模型并设置其位置和旋转
        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        // 获取枪械模型中的粒子系统组件
        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        ShootingAudioSource = Model.GetComponent<AudioSource>();
    }

    /// <summary>
    /// 执行射击逻辑，包括射速控制、散布计算、射线检测和弹道轨迹播放
    /// </summary>
    public void TryToShoot()
    {
        // 根据上次停止射击的时间与当前时间的间隔，调整初始点击时间以模拟后坐力恢复效果
        if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
        {
            float lastDuration = Mathf.Clamp(
                0,
                (StopShootingTIme - InitialClickTime),
                ShootConfig.MaxSpreadTime
            );
            float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTIme))
                / ShootConfig.RecoilRecoverySpeed;
            InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }
        
        // 检查是否满足射速间隔要求
        if (Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;
            // 检查弹药是否足够，如果不足则播放空仓音效并返回
            if (AmmoConfig.CurrentClipAmmo == 0)
            {
                AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                return;
            }
            
            // 播放射击特效和音效
            ShootSystem.Play();
            AudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);

            // 计算并应用射击散布方向偏移
            Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);
            Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
            
            Vector3 shootDirection = Vector3.zero;

            if (ShootConfig.ShootType == ShootType.FromGun)
            {
                shootDirection = ShootSystem.transform.forward;
            }
            else
            {
                shootDirection = ActiveCamera.transform.forward +
                                 ActiveCamera.transform.TransformDirection(shootDirection);
            }
            
            // 减少当前弹夹中的子弹数量
            AmmoConfig.CurrentClipAmmo--;

            // 根据配置选择命中检测方式：射线检测或投射物检测
            if (ShootConfig.IsHitscan)
            {
                DoHitscanShoot(shootDirection);
            }
            else
            {
                DoProjectileShoot(shootDirection);
            }
        }
    }

    /// <summary>
    /// 执行射线检测射击逻辑，根据是否命中目标播放对应的弹道轨迹。
    /// </summary>
    /// <param name="ShootDirection">射击方向向量</param>
    private void DoHitscanShoot(Vector3 ShootDirection)
    {
        
        // 发射射线检测命中
        if (Physics.Raycast(
                GetRaycastOrigin(),
                ShootDirection,
                out RaycastHit hit,
                float.MaxValue,
                ShootConfig.HitMask
            ))
        {
            // 命中目标时播放弹道轨迹
            ActiveMonoBehaviour.StartCoroutine(
                PlayTrail(
                    ShootSystem.transform.position,
                    hit.point,
                    hit
                )
            );
        }
        else
        {
            // 未命中目标时播放空弹道轨迹
            ActiveMonoBehaviour.StartCoroutine(
                PlayTrail(
                    ShootSystem.transform.position,
                    ShootSystem.transform.position + (ShootDirection * TrailConfig.MissDistance),
                    new RaycastHit()
                )
            );
        }
    }

    /// <summary>
    /// 获取射线检测的起点位置，根据射击类型决定是从枪口还是摄像机发出。
    /// </summary>
    /// <returns>射线检测的起点坐标</returns>
    public Vector3 GetRaycastOrigin()
    {
        Vector3 origin = ShootSystem.transform.position;
        
        if (ShootConfig.ShootType == ShootType.FromCamera)
        {
            origin = ActiveCamera.transform.position + ActiveCamera.transform.forward *
                Vector3.Distance(
                    ActiveCamera.transform.position, ShootSystem.transform.position
                );
        }
        
        return origin;
    }

    /// <summary>
    /// 获取枪械模型的前向方向。
    /// </summary>
    /// <returns>枪械模型的前向方向向量</returns>
    public Vector3 GetGunForward()
    {
        return Model.transform.forward;
    }

    /// <summary>
    /// 执行投射物射击逻辑，从对象池中获取子弹和尾迹，并初始化其状态。
    /// </summary>
    /// <param name="ShootDirection">射击方向向量</param>
    private void DoProjectileShoot(Vector3 ShootDirection)
    {
        Bullet bullet = BulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;

        if (ShootConfig.ShootType == ShootType.FromCamera
            && Physics.Raycast(
                GetRaycastOrigin(),
                ShootDirection,
                out RaycastHit hit,
                float.MaxValue,
                ShootConfig.HitMask))
        {
            Vector3 directionToHit = (hit.point - ShootSystem.transform.position).normalized;
            Model.transform.forward = directionToHit;
            ShootDirection = directionToHit;
        }
        
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(ShootDirection * ShootConfig.BulletSpawnForce);
        
        TrailRenderer trail = TrailPool.Get();
        if (trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 处理子弹碰撞事件，回收尾迹和子弹对象，并触发撞击效果。
    /// </summary>
    /// <param name="Bullet">发生碰撞的子弹实例</param>
    /// <param name="Collision">Unity 碰撞信息对象</param>
    private void HandleBulletCollision(Bullet Bullet, Collision Collision)
    {
        TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.transform.SetParent(null, true);
            ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
        }
        
        Bullet.gameObject.SetActive(false);
        BulletPool.Release(Bullet);

        if (Collision != null)
        {
            ContactPoint contactPoint = Collision.GetContact(0);

            HandleBulletImpact(
                Vector3.Distance(contactPoint.point, Bullet.SpawnLocation),
                contactPoint.point,
                contactPoint.normal,
                contactPoint.otherCollider
            );
        }
    }

    /// <summary>
    /// 处理子弹撞击逻辑，包括表面效果和对可伤害对象造成伤害。
    /// </summary>
    /// <param name="DistanceTraveled">子弹飞行的距离</param>
    /// <param name="HitLocation">撞击点坐标</param>
    /// <param name="HitNormal">撞击法线方向</param>
    /// <param name="HitCollider">被撞击的碰撞体</param>
    private void HandleBulletImpact(
        float DistanceTraveled,
        Vector3 HitLocation,
        Vector3 HitNormal,
        Collider HitCollider)
    {
        SurfaceManager.Instance.HandleImpact(
            HitCollider.gameObject,
            HitLocation,
            HitNormal,
            ImpactType,
            0
        );

        if (HitCollider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(DamageConfig.GetDamage(DistanceTraveled));
        }
    }

    /// <summary>
    /// 播放重新加载的音频剪辑（如果已分配）。
    /// </summary>
    public void StartReloading()
    {
        AudioConfig.PlayReloadClip(ShootingAudioSource);
    }
    
    /// <summary>
    /// 检查是否可以重新加载弹药
    /// </summary>
    /// <returns>如果可以重新加载则返回true，否则返回false</returns>
    public bool CanReload()
    {
        return AmmoConfig.CanReload();
    }

    /// <summary>
    /// 结束重新加载过程并执行实际的重新加载操作
    /// </summary>
    public void EndReload()
    {
        AmmoConfig.Reload();
    }

    /// <summary>
    /// 更新当前使用的摄像机引用。
    /// </summary>
    /// <param name="ActiveCamera">新的摄像机实例</param>
    public void UpdateCamera(Camera ActiveCamera)
    {
        this.ActiveCamera = ActiveCamera;
    }
    
    /// <summary>
    /// 每帧更新武器状态，处理射击逻辑和后坐力恢复
    /// </summary>
    /// <param name="WantsToShoot">是否想要射击的布尔值</param>
    public void Tick(bool WantsToShoot)
    {
        // 平滑恢复模型的旋转角度，模拟后坐力恢复效果
        Model.transform.localRotation = Quaternion.Lerp(
            Model.transform.localRotation,
            Quaternion.Euler(SpawnRotation),
            Time.deltaTime * ShootConfig.RecoilRecoverySpeed
        );
        
        if (WantsToShoot)
        {
            LastFrameWantedToShoot = true;
            TryToShoot();
        }
        else if (!WantsToShoot && LastFrameWantedToShoot)
        {
            StopShootingTIme = Time.time;
            LastFrameWantedToShoot = false;
        }
    }

    /// <summary>
    /// 播放轨迹效果，从起始点移动到终点，并处理碰撞效果
    /// </summary>
    /// <param name="StartPoint">轨迹的起始位置</param>
    /// <param name="EndPoint">轨迹的结束位置</param>
    /// <param name="Hit">射线检测的碰撞信息</param>
    /// <returns>协程枚举器</returns>
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        // 从对象池获取轨迹渲染器实例并初始化位置
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; // 避免复用上一帧的位置残留

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;

        // 动态更新轨迹位置直到到达终点
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        // 检查是否击中物体，如果击中则触发表面效果并处理伤害
        if (Hit.collider != null)
        {
            HandleBulletImpact(distance, EndPoint, Hit.normal, Hit.collider);
        }
        
        // 等待轨迹持续时间后回收对象
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    /// <summary>
    /// 延迟禁用轨迹渲染器的协程函数
    /// </summary>
    /// <param name="Trail">需要被延迟禁用的轨迹渲染器组件</param>
    /// <returns>返回IEnumerator用于协程执行</returns>
    private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
    {
        // 等待指定的持续时间后继续执行
        yield return new WaitForSeconds(TrailConfig.Duration);
        
        // 等待一帧确保渲染完成
        yield return null;
        
        // 停止轨迹渲染器的发射并禁用游戏对象，最后将轨迹渲染器回收到对象池
        Trail.emitting = false;
        Trail.gameObject.SetActive(false);
        TrailPool.Release(Trail);
    }

    /// <summary>
    /// 创建一个新的 TrailRenderer 实例作为子弹轨迹使用。
    /// </summary>
    /// <returns>新创建的 TrailRenderer 组件实例。</returns>
    private TrailRenderer CreateTrail()
    {
        // 创建子弹轨迹的游戏对象
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        
        // 配置轨迹渲染器的基本属性
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        // 禁用初始发射状态并关闭阴影投射
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    /// <summary>
    /// 创建一个新的 Bullet 实例作为子弹投射物使用。
    /// </summary>
    /// <returns>新创建的 Bullet 组件实例。</returns>
    private Bullet CreateBullet()
    {
        return Instantiate(ShootConfig.BulletPrefab);
    }

    /// <summary>
    /// 创建当前枪械配置对象的深拷贝副本
    /// </summary>
    /// <returns>返回一个新的GunScriptableObject实例，包含与当前对象相同的所有配置数据</returns>
    public object Clone()
    {
        // 创建新的枪械配置对象实例
        GunScriptableObject config = CreateInstance<GunScriptableObject>();

        // 复制基础配置信息
        config.ImpactType = ImpactType;
        config.Type = Type;
        config.Name = Name;
        config.name = name;
        
        // 递归克隆各个配置子对象
        config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
        config.ShootConfig = ShootConfig.Clone() as ShootConfigScriptableObject;
        config.AmmoConfig = AmmoConfig.Clone() as AmmoConfigScriptableObject;
        config.TrailConfig = TrailConfig.Clone() as TrailConfigScriptableObject;
        config.AudioConfig = AudioConfig.Clone() as AudioConfigScriptableObject;
        
        // 复制模型和位置配置
        config.ModelPrefab = ModelPrefab;
        config.SpawnPoint = SpawnPoint;
        config.SpawnRotation = SpawnRotation;
        
        return config;
    }

}
