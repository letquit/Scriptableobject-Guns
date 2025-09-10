using UnityEngine;

/// <summary>
/// ImpactType类用于定义游戏中的冲击类型数据
/// 该类继承自ScriptableObject，可以在Unity编辑器中创建为资产文件
/// 通过CreateAssetMenu属性，可以在Assets/Create/Impact System菜单下创建Impact Type资产
/// </summary>
[CreateAssetMenu(menuName = "Impact System/Impact Type", fileName = "ImpactType")]
public class ImpactType : ScriptableObject
{

}

