using Map;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public BlueprintObj levelInfo;


    public void Init(BlueprintObj levelInfo)
    {
        this.levelInfo = levelInfo;

        if (levelInfo.info.nodeType == NodeType.��ͨ�ַ� || levelInfo.info.nodeType == NodeType.��Ӣ�ַ� ||
            levelInfo.info.nodeType == NodeType.Boss��)
        {

        }
    }
}
