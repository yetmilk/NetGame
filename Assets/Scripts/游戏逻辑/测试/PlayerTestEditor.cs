using UnityEngine;

public class PlayerTestEditor : MonoBehaviour
{
    [Header("�Ƿ��ǵ���ģʽ")]
    public bool isTest;

    [Header("Ҫ���ԵĽ�ɫ")]
    public CharacterClacify character;

    private void Start()
    {
        if (isTest)
        {
           // SpawnCharacter();
        }
    }

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

    /// <summary>
    /// ��Inspector�Ҽ��˵������"���ɽ�ɫ"��ť
    /// </summary>
    [ContextMenu("���ɽ�ɫ")]
    public void SpawnCharacterFromButton()
    {
        SpawnCharacter();
    }

    /// <summary>
    /// ��Inspector����ʾһ����ť
    /// ��Ҫ����UnityEditor�����ռ�
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(PlayerTestEditor))]
    public class PlayerTestEditorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // ����Ĭ�ϵ�Inspector
            DrawDefaultInspector();

            // ��ȡ��ǰ�ű�ʵ��
            PlayerTestEditor testEditor = (PlayerTestEditor)target;

            // ���һ����ť
            if (GUILayout.Button("���ɽ�ɫ"))
            {
                testEditor.SpawnCharacter();
            }
        }
    }
#endif
}
