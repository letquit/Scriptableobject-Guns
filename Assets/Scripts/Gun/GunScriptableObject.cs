using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 枪械数据的 ScriptableObject 类，用于定义枪械的基本属性、射击配置和弹道特效。
/// </summary>
[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
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
    private AudioSource ShootingAudioSource;
    private float LastShootTime;
    private float InitialClickTime;
    private float StopShootingTIme;
    private bool LastFrameWantedToShoot;
    
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;

    /// <summary>
    /// 初始化枪械模型并绑定到指定父对象上。
    /// </summary>
    /// <param name="Parent">枪械模型的父级 Transform。</param>
    /// <param name="ActiveMonoBehaviour">当前激活的 MonoBehaviour 实例，用于协程启动。</param>
    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        
        // 重置射击相关的时间参数和弹药配置
        LastShootTime = 0; // 在编辑器中，这将不会被正确重置，在构建中没问题
        StopShootingTIme = 0;
        InitialClickTime = 0;
        AmmoConfig.CurrentClipAmmo = AmmoConfig.ClipSize;
        AmmoConfig.CurrentAmmo = AmmoConfig.MaxAmmo;
        
        // 创建子弹轨迹渲染器的对象池
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

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
            if (AmmoConfig.CurrentClipAmmo == 0)
            {
                AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                return;
            }
            
            ShootSystem.Play();
            AudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);

            // 计算并应用射击散布方向偏移
            Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);
            Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
            
            Vector3 shootDirection = Model.transform.forward;

            AmmoConfig.CurrentClipAmmo--;
            // 发射射线检测命中
            if (Physics.Raycast(
                    ShootSystem.transform.position,
                    shootDirection,
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
                        ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                        new RaycastHit()
                    )
                );
            }
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
            // 调用表面管理器处理撞击效果
            SurfaceManager.Instance.HandleImpact(
                Hit.transform.gameObject,
                EndPoint,
                Hit.normal,
                ImpactType,
                0
            );

            // 检查被击中的物体是否实现了IDamageable接口，如果是则应用伤害
            if (Hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(distance));
            }
        }


        // 等待轨迹持续时间后回收对象
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
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


}
