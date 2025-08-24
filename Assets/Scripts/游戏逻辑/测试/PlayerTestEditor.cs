using UnityEngine;

public class PlayerTestEditor : MonoBehaviour
{
    [Header("是否是调试模式")]
    public bool isTest;

    [Header("要测试的角色")]
    public CharacterClacify character;

    private void Start()
    {
        if (isTest)
        {
           // SpawnCharacter();
        }
    }

    /// <summary>
    /// 生成角色的方法
    /// </summary>
    private void SpawnCharacter()
    {
        if (LoadManager.Instance != null)
        {
            
                LoadManager.Instance.NetInstantiate(character.ToString());
            
           
        }
        else
        {
            Debug.LogError("LoadManager实例不存在!");
        }
    }

    /// <summary>
    /// 在Inspector右键菜单中添加"生成角色"按钮
    /// </summary>
    [ContextMenu("生成角色")]
    public void SpawnCharacterFromButton()
    {
        SpawnCharacter();
    }

    /// <summary>
    /// 在Inspector中显示一个按钮
    /// 需要引入UnityEditor命名空间
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(PlayerTestEditor))]
    public class PlayerTestEditorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认的Inspector
            DrawDefaultInspector();

            // 获取当前脚本实例
            PlayerTestEditor testEditor = (PlayerTestEditor)target;

            // 添加一个按钮
            if (GUILayout.Button("生成角色"))
            {
                testEditor.SpawnCharacter();
            }
        }
    }
#endif
}
