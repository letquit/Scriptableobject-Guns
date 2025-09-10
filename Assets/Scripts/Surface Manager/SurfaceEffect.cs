using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SurfaceEffect类用于定义表面效果配置，继承自ScriptableObject
/// 该类允许通过Unity编辑器创建表面效果资产，包含生成对象效果和播放音频效果的配置列表
/// </summary>
[CreateAssetMenu(menuName = "Impact System/Surface Effect", fileName = "SurfaceEffect")]
public class SurfaceEffect : ScriptableObject
{
    /// <summary>
    /// 生成对象效果列表，用于存储当表面效果触发时需要生成的游戏对象效果配置
    /// </summary>
    public List<SpawnObjectEffect> SpawnObjectEffects = new List<SpawnObjectEffect>();
    
    /// <summary>
    /// 播放音频效果列表，用于存储当表面效果触发时需要播放的音频效果配置
    /// </summary>
    public List<PlayAudioEffect> PlayAudioEffects = new List<PlayAudioEffect>();
}
