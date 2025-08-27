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

        if (levelInfo.info.nodeType == NodeType.普通怪房 || levelInfo.info.nodeType == NodeType.精英怪房 ||
            levelInfo.info.nodeType == NodeType.Boss房)
        {

        }
    }
}
