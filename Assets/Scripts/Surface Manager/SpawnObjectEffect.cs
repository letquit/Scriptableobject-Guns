using UnityEngine;

/// <summary>
/// SpawnObjectEffect类用于定义生成对象效果的配置数据
/// 该类继承自ScriptableObject，可以在Unity编辑器中创建资产文件进行配置
/// </summary>
[CreateAssetMenu(menuName = "Impact System/Spawn Object Effect", fileName = "SpawnObjectEffect")]
public class SpawnObjectEffect : ScriptableObject
{
    /// <summary>
    /// 要生成的预制体对象
    /// </summary>
    public GameObject Prefab;
    
    /// <summary>
    /// 生成概率，范围从0到1，1表示100%生成
    /// </summary>
    public float Probability = 1;
    
    /// <summary>
    /// 是否随机化旋转
    /// </summary>
    public bool RandomizeRotation;
    
    /// <summary>
    /// 随机旋转乘数，用于控制各轴的旋转范围
    /// 零值将锁定该轴的旋转，每个X,Y,Z轴的值建议设置为0到360之间
    /// </summary>
    [Tooltip("Zero values will lock the rotation on that axis. Values up to 360 are sensible for each X,Y,Z")]
    public Vector3 RandomizedRotationMultiplier = Vector3.zero;
}

