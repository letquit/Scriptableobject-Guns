using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 射击配置脚本化对象类
/// 用于存储和管理武器射击的相关配置参数
/// 可以通过Unity编辑器的菜单创建该资源文件
/// </summary>
[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigScriptableObject : ScriptableObject, ICloneable
{
    /// <summary>
    /// 是否启用瞬间命中检测模式
    /// </summary>
    public bool IsHitscan = true;

    /// <summary>
    /// 子弹预制体引用
    /// </summary>
    public Bullet BulletPrefab;

    /// <summary>
    /// 子弹生成时的初始推力大小
    /// </summary>
    public float BulletSpawnForce = 1000f;

    /// <summary>
    /// 子弹命中的层级掩码
    /// 用于确定子弹可以与哪些层级的物体发生碰撞检测
    /// </summary>
    public LayerMask HitMask;
    
    /// <summary>
    /// 射击频率
    /// 控制连续射击时每发子弹之间的间隔时间(秒)
    /// 值越小射速越快，默认值0.25秒
    /// </summary>
    public float FireRate = 0.25f;

    public int BulletsPerShot = 1;
    
    /// <summary>
    /// 射击类型枚举值，表示当前使用的射击方式
    /// </summary>
    public ShootType ShootType = ShootType.FromGun;
    
    /// <summary>
    /// 后坐力恢复速度
    /// 控制射击后武器后坐力恢复到初始状态的速度
    /// 值越大恢复越快
    /// </summary>
    public float RecoilRecoverySpeed = 1f;

    /// <summary>
    /// 最大散布时间
    /// 控制子弹散布效果达到最大值所需的时间(秒)
    /// 用于模拟长时间射击时精度下降的效果
    /// </summary>
    public float MaxSpreadTime = 1f;
    
    /// <summary>
    /// 子弹散布类型
    /// 定义子弹发射时的散布模式
    /// Simple: 简单随机散布
    /// </summary>
    public BulletSpreadType SpreadType = BulletSpreadType.Simple;

    
    /// <summary>
    /// 射击散布向量
    /// 控制子弹发射时的随机散布范围，xyz分量分别对应不同方向的散布程度
    /// 默认值为(0.1, 0.1, 0.1)
    /// </summary>
    [Header("Simple Spread")]
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    
    public Vector3 MinSpread = Vector3.zero;

    /// <summary>
    /// 基于纹理的散布设置
    /// 通过纹理数据来控制射击散布的复杂模式
    /// </summary>
    [Header("Texture-Based Spread")]
    [Range(0.001f, 5f)]
    public float SpreadMultiplier = 0.1f;
    
    /// <summary>
    /// 散布控制纹理
    /// 用于定义基于纹理坐标的散布模式，纹理的RGB值会影响子弹的散布方向和程度
    /// </summary>
    public Texture2D SpreadTexture;

    
    /// <summary>
    /// 获取当前射击的散布向量，用于控制子弹发射时的方向偏移。
    /// 根据 SpreadType 的类型选择不同的散布计算方式：
    /// - Simple 类型使用简单的三轴随机偏移；
    /// - TextureBased 类型则基于纹理灰度进行加权随机采样，生成方向向量。
    /// </summary>
    /// <param name="ShootTime">射击持续时间，用于插值计算散布强度，默认为0。</param>
    /// <returns>表示子弹散布方向的 Vector3 向量。</returns>
    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 spread = Vector3.zero;

        // 简单散布模式：在 XYZ 三个方向上根据 Spread 向量进行随机偏移，并随时间插值
        if (SpreadType == BulletSpreadType.Simple)
        {
            spread = Vector3.Lerp(
                new Vector3(
                    Random.Range(-MinSpread.x, MinSpread.x),
                    Random.Range(-MinSpread.y, MinSpread.y),
                    Random.Range(-MinSpread.z, MinSpread.z)
                ), 
                new Vector3(
                    Random.Range(-Spread.x, Spread.x),
                    Random.Range(-Spread.y, Spread.y),
                    Random.Range(-Spread.z, Spread.z)
                ), 
                Mathf.Clamp01(ShootTime / MaxSpreadTime)
            );
        }
        // 基于纹理的散布模式：使用纹理灰度加权采样获取方向，并乘以倍数调整强度
        else if (SpreadType == BulletSpreadType.TextureBased)
        {
            spread = GetTextureDirection(ShootTime);
            spread *= SpreadMultiplier;
        }

        return spread;
    }

    /// <summary>
    /// 使用基于纹理灰度的加权随机算法，从 SpreadTexture 中采样一个方向向量。
    /// 灰度值越高的像素被选中的概率越大，实现更自然的散布分布。
    /// </summary>
    /// <param name="ShootTime">射击持续时间，用于动态调整采样区域大小。</param>
    /// <returns>归一化到 [-1, 1] 范围内的二维方向向量（z 分量为 0）。</returns>
    private Vector3 GetTextureDirection(float ShootTime)
    {
        // 计算纹理中心和当前采样区域半径
        Vector2 halfSize = new Vector2(SpreadTexture.width / 2f, SpreadTexture.height / 2f);
        int halfSquareExtents = Mathf.CeilToInt(
            Mathf.Lerp(
                0.01f,
                halfSize.x,
                Mathf.Clamp01(ShootTime / MaxSpreadTime)
            )
        );

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        // 从纹理中提取当前采样区域的所有颜色
        Color[] sampleColors = SpreadTexture.GetPixels(
            minX,
            minY,
            halfSquareExtents * 2,
            halfSquareExtents * 2
        );

        // 将颜色转换为灰度值并计算总和，用于后续加权随机选择
        float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGreyValue = colorsAsGrey.Sum();

        // 加权随机选取一个像素
        float grey = Random.Range(0, totalGreyValue);
        int i = 0;

        for (; i < colorsAsGrey.Length; i++)
        {
            grey -= colorsAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        // 计算选中像素在纹理中的坐标
        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        // 将像素坐标转换为中心对齐的归一化方向向量
        Vector2 targetPosition = new Vector2(x, y);
        Vector2 direction = (targetPosition - halfSize) / halfSize.x;

        return direction;
    }

    /// <summary>
    /// 创建当前射击配置脚本化对象的克隆副本
    /// </summary>
    /// <returns>返回一个新的ShootConfigScriptableObject对象，包含与当前对象相同的数据</returns>
    public object Clone()
    {
        // 创建新的脚本化对象实例
        ShootConfigScriptableObject config = CreateInstance<ShootConfigScriptableObject>();
        
        // 复制当前对象的所有值到新创建的实例中
        Utilities.CopyValues(this, config);
        
        return config;
    }

}

