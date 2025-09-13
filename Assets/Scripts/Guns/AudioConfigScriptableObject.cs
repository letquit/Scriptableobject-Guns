using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 音频配置脚本化对象类，用于管理枪械相关的音频配置和播放
/// 通过CreateAssetMenu可以在Unity编辑器中创建该配置文件的实例
/// </summary>
[CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
public class AudioConfigScriptableObject : ScriptableObject, ICloneable
{
    /// <summary>
    /// 音量控制，范围在0到1之间
    /// </summary>
    [Range(0, 1f)]
    public float Volume = 1f;

    /// <summary>
    /// 射击音效数组，支持多个音效随机播放
    /// </summary>
    public AudioClip[] FireClips;
    
    /// <summary>
    /// 空弹匣音效
    /// </summary>
    public AudioClip EmptyCLip;
    
    /// <summary>
    /// 装弹音效
    /// </summary>
    public AudioClip ReloadClip;
    
    /// <summary>
    /// 最后一颗子弹音效
    /// </summary>
    public AudioClip LastBulletClip;

    /// <summary>
    /// 播放射击音效
    /// </summary>
    /// <param name="AudioSource">用于播放音效的音频源组件</param>
    /// <param name="IsLastBullet">是否为最后一颗子弹，默认为false</param>
    public void PlayShootingClip(AudioSource AudioSource, bool IsLastBullet = false)
    {
        // 根据是否为最后一颗子弹选择不同的音效播放
        if (IsLastBullet && LastBulletClip != null)
        {
            AudioSource.PlayOneShot(LastBulletClip, Volume);
        }
        else
        {
            AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
        }
    }

    /// <summary>
    /// 播放空弹匣音效
    /// </summary>
    /// <param name="AudioSource">用于播放音效的音频源组件</param>
    public void PlayOutOfAmmoClip(AudioSource AudioSource)
    {
        // 检查音效是否存在后播放
        if (EmptyCLip != null)
        {
            AudioSource.PlayOneShot(EmptyCLip, Volume);
        }
    }
    
    /// <summary>
    /// 播放装弹音效
    /// </summary>
    /// <param name="AudioSource">用于播放音效的音频源组件</param>
    public void PlayReloadClip(AudioSource AudioSource)
    {
        // 检查音效是否存在后播放
        if (ReloadClip != null)
        {
            AudioSource.PlayOneShot(ReloadClip, Volume);
        }
    }
    
    /// <summary>
    /// 创建当前音频配置脚本化对象的深层副本
    /// </summary>
    /// <returns>返回一个新的AudioConfigScriptableObject实例，包含与当前对象相同的数据</returns>
    public object Clone()
    {
        // 创建新的音频配置对象实例
        AudioConfigScriptableObject config = CreateInstance<AudioConfigScriptableObject>();
        
        // 将当前对象的所有值复制到新创建的实例中
        Utilities.CopyValues(this, config);
        
        return config;
    }

}
