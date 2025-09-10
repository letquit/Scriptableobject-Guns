using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音频效果播放配置类，用于定义和管理音频播放的相关参数
/// 继承自ScriptableObject，可以在Unity编辑器中创建资产文件进行配置
/// </summary>
[CreateAssetMenu(menuName = "Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
public class PlayAudioEffect : ScriptableObject
{
    /// <summary>
    /// 音频源预制体，用于实例化音频播放组件
    /// </summary>
    public AudioSource AudioSourcePrefab;
    
    /// <summary>
    /// 可播放的音频剪辑列表，支持多个音频文件的随机播放
    /// </summary>
    public List<AudioClip> AudioClips = new List<AudioClip>();
    
    /// <summary>
    /// 音量范围设置，用于随机化音频播放的音量
    /// X分量表示最小音量，Y分量表示最大音量
    /// 音量值会被限制在0-1范围内
    /// </summary>
    [Tooltip("Values are clamped to 0-1")]
    public Vector2 VolumeRange = new Vector2(0, 1);
}
