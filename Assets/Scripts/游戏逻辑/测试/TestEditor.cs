using Map;
using UnityEngine;

public class TestEditor : MonoBehaviour
{
    [Header("�Ƿ��ǵ���ģʽ")]
    public bool isTest;

    [Header("Ҫ���ԵĽ�ɫ")]
    public CharacterClacify character;

    [Header("Ҫ�л��ĳ���")]
    public SceneName sceneName;

    [Header("Ҫ�л���Buff")]
    public BuffName BuffName;

    private void Start()
    {
        if (isTest)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    #region -----------------------���ɽ�ɫ------------------------------
    /// <summary>
    /// ���ɽ�ɫ�ķ���
    /// </summary>
    private void SpawnCharacter()
    {
        if (LoadManager.Instance != null)
        {
            LoadManager.Instance.NetInstantiate(character.ToString());
        }
        else
        {
            Debug.LogError("LoadManagerʵ��������!");
        }
    }
    public void SpawnCharacterFromButton()
    {
        SpawnCharacter();
    }
    #endregion


    #region--------------------�л�����------------------------
    public void TransToScene()
    {
        GameSceneManager.Instance.LoadSceneToServer(sceneName);
    }
    #endregion

    #region----------------�����ؿ�--------------------
    public void NextLevel()
    {
        BattleManager.Instance.MapManager.GoToNextLevel();
    }
    #endregion

    #region ------------------�л�Buff-------------

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

            // ����isTest�������·��ĵ���ģʽ˵��
            testEditor.isTest = UnityEditor.EditorGUILayout.Toggle(new GUIContent("�Ƿ��ǵ���ģʽ", "��������ģʽ����󲻻ᱻ����"), testEditor.isTest);
            GUILayout.Space(5);

            // ���ƽ�ɫ�������·������ɽ�ɫ��ť
            UnityEditor.EditorGUILayout.LabelField("Ҫ���ԵĽ�ɫ", UnityEditor.EditorStyles.boldLabel);
            testEditor.character = (CharacterClacify)UnityEditor.EditorGUILayout.EnumPopup(testEditor.character);
            if (GUILayout.Button("���ɽ�ɫ"))
            {
                testEditor.SpawnCharacter();
            }
            GUILayout.Space(10);

            // ���Ƴ����������·����л�������ť
            UnityEditor.EditorGUILayout.LabelField("Ҫ�л��ĳ���", UnityEditor.EditorStyles.boldLabel);
            testEditor.sceneName = (SceneName)UnityEditor.EditorGUILayout.EnumPopup(testEditor.sceneName);
            if (GUILayout.Button("�л�����"))
            {
                testEditor.TransToScene();
            }
            GUILayout.Space(10);

            // ���ƿ����ؿ����ּ���ť
            UnityEditor.EditorGUILayout.LabelField("�ؿ�����", UnityEditor.EditorStyles.boldLabel);
            if (GUILayout.Button("��һ��"))
            {
                testEditor.NextLevel();
            }
            GUILayout.Space(10);

            UnityEditor.EditorGUILayout.LabelField("���buff", UnityEditor.EditorStyles.boldLabel);
            testEditor.BuffName = (BuffName)UnityEditor.EditorGUILayout.EnumPopup(testEditor.BuffName);
            if (GUILayout.Button("���buff"))
            {
                testEditor.ChangeBuff();
            }
            GUILayout.Space(10);
            // Ӧ���޸�
            if (GUI.changed)
            {
                UnityEditor.EditorUtility.SetDirty(testEditor);
            }
        }
    }
#endif
}
