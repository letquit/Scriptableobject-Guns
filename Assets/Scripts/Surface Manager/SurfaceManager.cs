using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 管理场景中表面类型及其对应的冲击效果。该类为单例，负责根据碰撞对象的材质或地形纹理，
/// 播放对应的视觉和音频效果。
/// </summary>
public class SurfaceManager : MonoBehaviour
{
    private static SurfaceManager _instance;
    
    /// <summary>
    /// 获取 SurfaceManager 的单例实例。
    /// </summary>
    public static SurfaceManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        // 确保场景中只有一个 SurfaceManager 实例
        if (Instance != null)
        {
            Debug.LogError("More than one SurfaceManager active in the scene! Destroying latest one: " + name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField]
    private List<SurfaceType> Surfaces = new List<SurfaceType>();
    [SerializeField]
    private Surface DefaultSurface;
    private Dictionary<GameObject, ObjectPool<GameObject>> ObjectPools = new();

    /// <summary>
    /// 处理一次冲击事件，根据碰撞对象的材质或地形纹理播放对应的视觉和音频效果。
    /// </summary>
    /// <param name="HitObject">被击中的游戏对象。</param>
    /// <param name="HitPoint">碰撞点的世界坐标。</param>
    /// <param name="HitNormal">碰撞点的法线方向。</param>
    /// <param name="Impact">冲击类型。</param>
    /// <param name="TriangleIndex">碰撞的三角面索引（用于 MeshRenderer）。</param>
    public void HandleImpact(GameObject HitObject, Vector3 HitPoint, Vector3 HitNormal, ImpactType Impact, int TriangleIndex)
    {
        // 判断是否为 Terrain 对象
        if (HitObject.TryGetComponent<Terrain>(out Terrain terrain))
        {
            // 获取地形上碰撞点处激活的纹理
            List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, HitPoint);
            foreach (TextureAlpha activeTexture in activeTextures)
            {
                // 查找匹配的 SurfaceType
                SurfaceType surfaceType = Surfaces.Find(surface => surface.Albedo == activeTexture.Texture);
                if (surfaceType != null)
                {
                    // 遍历该 SurfaceType 中匹配 ImpactType 的效果并播放
                    foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == Impact)
                        {
                            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, activeTexture.Alpha);
                        }
                    }
                }
                else
                {
                    // 使用默认 Surface 播放效果
                    foreach (Surface.SurfaceImpactTypeEffect typeEffect in DefaultSurface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == Impact)
                        {
                            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
                        }
                    }
                }
            }
        }
        // 判断是否为普通 Renderer 对象
        else if (HitObject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            // 获取 Renderer 上碰撞点对应的纹理
            Texture activeTexture = GetActiveTextureFromRenderer(renderer, TriangleIndex);

            // 查找匹配的 SurfaceType
            SurfaceType surfaceType = Surfaces.Find(surface => surface.Albedo == activeTexture);
            if (surfaceType != null)
            {
                foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                {
                    if (typeEffect.ImpactType == Impact)
                    {
                        PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
                    }
                }
            }
            else
            {
                // 使用默认 Surface 播放效果
                foreach (Surface.SurfaceImpactTypeEffect typeEffect in DefaultSurface.ImpactTypeEffects)
                {
                    if (typeEffect.ImpactType == Impact)
                    {
                        PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 从 Terrain 对象中获取碰撞点处激活的纹理及其权重。
    /// </summary>
    /// <param name="Terrain">地形对象。</param>
    /// <param name="HitPoint">碰撞点的世界坐标。</param>
    /// <returns>包含纹理和权重的 TextureAlpha 列表。</returns>
    private List<TextureAlpha> GetActiveTexturesFromTerrain(Terrain Terrain, Vector3 HitPoint)
    {
        Vector3 terrainPosition = HitPoint - Terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(
            terrainPosition.x / Terrain.terrainData.size.x,
            0,
            terrainPosition.z / Terrain.terrainData.size.z
        );

        int x = Mathf.FloorToInt(splatMapPosition.x * Terrain.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(splatMapPosition.z * Terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = Terrain.terrainData.GetAlphamaps(x, z, 1, 1);

        List<TextureAlpha> activeTextures = new List<TextureAlpha>();
        for (int i = 0; i < alphaMap.Length; i++)
        {
            if (alphaMap[0, 0, i] > 0)
            {
                activeTextures.Add(new TextureAlpha()
                {
                    Texture = Terrain.terrainData.terrainLayers[i].diffuseTexture,
                    Alpha = alphaMap[0, 0, i]
                });
            }
        }

        return activeTextures;
    }

    /// <summary>
    /// 根据 Renderer 和三角面索引获取对应的主纹理。
    /// </summary>
    /// <param name="Renderer">渲染器组件。</param>
    /// <param name="TriangleIndex">碰撞的三角面索引。</param>
    /// <returns>对应的主纹理，若无法获取则返回 null。</returns>
    private Texture GetActiveTextureFromRenderer(Renderer Renderer, int TriangleIndex)
    {
        if (Renderer.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            Mesh mesh = meshFilter.mesh;

            if (mesh.subMeshCount > 1)
            {
                int[] hitTriangleIndices = new int[]
                {
                    mesh.triangles[TriangleIndex * 3],
                    mesh.triangles[TriangleIndex * 3 + 1],
                    mesh.triangles[TriangleIndex * 3 + 2]
                };

                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    int[] submeshTriangles = mesh.GetTriangles(i);
                    for (int j = 0; j < submeshTriangles.Length; j += 3)
                    {
                        if (submeshTriangles[j] == hitTriangleIndices[0]
                            && submeshTriangles[j + 1] == hitTriangleIndices[1]
                            && submeshTriangles[j + 2] == hitTriangleIndices[2])
                        {
                            return Renderer.sharedMaterials[i].mainTexture;
                        }
                    }
                }
            }
            else
            {
                return Renderer.sharedMaterial.mainTexture;
            }
        }

        Debug.LogError($"{Renderer.name} has no MeshFilter! Using default impact effect instead of texture-specific one because we'll be unable to find the correct texture!");
        return null;
    }

    /// <summary>
    /// 播放指定的 SurfaceEffect，包括生成对象和播放音频。
    /// </summary>
    /// <param name="HitPoint">碰撞点的世界坐标。</param>
    /// <param name="HitNormal">碰撞点的法线方向。</param>
    /// <param name="SurfaceEffect">要播放的 SurfaceEffect 数据。</param>
    /// <param name="SoundOffset">音频音量的缩放因子。</param>
    private void PlayEffects(Vector3 HitPoint, Vector3 HitNormal, SurfaceEffect SurfaceEffect, float SoundOffset)
    {
        // 播放 SpawnObjectEffect
        foreach (SpawnObjectEffect spawnObjectEffect in SurfaceEffect.SpawnObjectEffects)
        {
            if (spawnObjectEffect.Probability > Random.value)
            {
                if (!ObjectPools.ContainsKey(spawnObjectEffect.Prefab))
                {
                    ObjectPools.Add(spawnObjectEffect.Prefab, new ObjectPool<GameObject>(() => Instantiate(spawnObjectEffect.Prefab)));
                }

                GameObject instance = ObjectPools[spawnObjectEffect.Prefab].Get();

                if (instance.TryGetComponent(out PoolableObject poolable))
                {
                    poolable.Parent = ObjectPools[spawnObjectEffect.Prefab];
                }
                instance.SetActive(true);
                instance.transform.position = HitPoint + HitNormal * 0.001f;
                instance.transform.forward = HitNormal;

                if (spawnObjectEffect.RandomizeRotation)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.x),
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.y),
                        Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.z)
                    );

                    instance.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + offset);
                }
            }
        }

        // 播放 PlayAudioEffect
        foreach (PlayAudioEffect playAudioEffect in SurfaceEffect.PlayAudioEffects)
        {
            if (!ObjectPools.ContainsKey(playAudioEffect.AudioSourcePrefab.gameObject))
            {
                ObjectPools.Add(playAudioEffect.AudioSourcePrefab.gameObject, new ObjectPool<GameObject>(() => Instantiate(playAudioEffect.AudioSourcePrefab.gameObject)));
            }

            AudioClip clip = playAudioEffect.AudioClips[Random.Range(0, playAudioEffect.AudioClips.Count)];
            GameObject instance = ObjectPools[playAudioEffect.AudioSourcePrefab.gameObject].Get();
            instance.SetActive(true);
            AudioSource audioSource = instance.GetComponent<AudioSource>();

            audioSource.transform.position = HitPoint;
            audioSource.PlayOneShot(clip, SoundOffset * Random.Range(playAudioEffect.VolumeRange.x, playAudioEffect.VolumeRange.y));
            StartCoroutine(DisableAudioSource(ObjectPools[playAudioEffect.AudioSourcePrefab.gameObject], audioSource, clip.length));
        }
    }

    /// <summary>
    /// 在音频播放完成后，将 AudioSource 对象回收到对象池。
    /// </summary>
    /// <param name="Pool">对象池。</param>
    /// <param name="AudioSource">要回收的 AudioSource 组件。</param>
    /// <param name="Time">等待的时间（音频长度）。</param>
    /// <returns>协程。</returns>
    private IEnumerator DisableAudioSource(ObjectPool<GameObject> Pool, AudioSource AudioSource, float Time)
    {
        yield return new WaitForSeconds(Time);

        AudioSource.gameObject.SetActive(false);
        Pool.Release(AudioSource.gameObject);
    }

    /// <summary>
    /// 用于存储地形纹理及其权重的辅助类。
    /// </summary>
    private class TextureAlpha
    {
        public float Alpha;
        public Texture Texture;
    }
}
