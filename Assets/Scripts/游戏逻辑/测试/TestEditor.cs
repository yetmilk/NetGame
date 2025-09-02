using Map;
using UnityEngine;

public class TestEditor : MonoBehaviour
{
    [Header("是否是调试模式")]
    public bool isTest;

    [Header("要测试的角色")]
    public CharacterClacify character;

    [Header("要切换的场景")]
    public SceneName sceneName;

    [Header("要切换的Buff")]
    public BuffName BuffName;

    private void Start()
    {
        if (isTest)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    #region -----------------------生成角色------------------------------
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
    public void SpawnCharacterFromButton()
    {
        SpawnCharacter();
    }
    #endregion


    #region--------------------切换场景------------------------
    public void TransToScene()
    {
        GameSceneManager.Instance.LoadSceneToServer(sceneName);
    }
    #endregion

    #region----------------开启关卡--------------------
    public void NextLevel()
    {
        BattleManager.Instance.MapManager.GoToNextLevel();
    }
    #endregion

    #region ------------------切换Buff-------------

    public void ChangeBuff()
    {
        CharacterController character = PlayerManager.Instance.curActivePlayer[PlayerManager.Instance.selfId].playerObj;
        AddBuffInfo buffInfo = new AddBuffInfo(BuffName.ToString(), character, character);
        character.curCharaData.buffController.AddBuff(buffInfo);
    }
    #endregion


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TestEditor))]
    public class TestEditorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TestEditor testEditor = (TestEditor)target;

            // 绘制isTest变量及下方的调试模式说明
            testEditor.isTest = UnityEditor.EditorGUILayout.Toggle(new GUIContent("是否是调试模式", "开启调试模式后对象不会被销毁"), testEditor.isTest);
            GUILayout.Space(5);

            // 绘制角色变量及下方的生成角色按钮
            UnityEditor.EditorGUILayout.LabelField("要测试的角色", UnityEditor.EditorStyles.boldLabel);
            testEditor.character = (CharacterClacify)UnityEditor.EditorGUILayout.EnumPopup(testEditor.character);
            if (GUILayout.Button("生成角色"))
            {
                testEditor.SpawnCharacter();
            }
            GUILayout.Space(10);

            // 绘制场景变量及下方的切换场景按钮
            UnityEditor.EditorGUILayout.LabelField("要切换的场景", UnityEditor.EditorStyles.boldLabel);
            testEditor.sceneName = (SceneName)UnityEditor.EditorGUILayout.EnumPopup(testEditor.sceneName);
            if (GUILayout.Button("切换场景"))
            {
                testEditor.TransToScene();
            }
            GUILayout.Space(10);

            // 绘制开启关卡部分及按钮
            UnityEditor.EditorGUILayout.LabelField("关卡控制", UnityEditor.EditorStyles.boldLabel);
            if (GUILayout.Button("下一关"))
            {
                testEditor.NextLevel();
            }
            GUILayout.Space(10);

            UnityEditor.EditorGUILayout.LabelField("添加buff", UnityEditor.EditorStyles.boldLabel);
            testEditor.BuffName = (BuffName)UnityEditor.EditorGUILayout.EnumPopup(testEditor.BuffName);
            if (GUILayout.Button("添加buff"))
            {
                testEditor.ChangeBuff();
            }
            GUILayout.Space(10);
            // 应用修改
            if (GUI.changed)
            {
                UnityEditor.EditorUtility.SetDirty(testEditor);
            }
        }
    }
#endif
}
