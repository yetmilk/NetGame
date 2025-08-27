using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour, ITip
{
    public  Transform insParent;

    public void Close()
    {
        
    }

    public void Show()
    {
        for (int i = 0; i < insParent.childCount; i++)
        {
            Destroy(insParent.GetChild(i).gameObject);
        }
        var infos = MapManager.Instance.CurrentMap.GetLayerNodes(MapManager.Instance.curProgress.level);
        var obj = LoadManager.Instance.GetResourceByName<GameObject>("¹Ø¿¨Ñ¡Ôñ²Û");

        for (int i = 0; i < infos.Length; i++)
        {
            var item = Instantiate(obj, insParent);
            if (infos[i].blueprintObj is EnemyLevelObj enemyObj)
            {
                item.GetComponent<LevelSelectObj>().Init(i, infos[i].nodeType.ToString(), enemyObj.rewardType.ToString(), enemyObj.elementType.ToString());
            }
            else
                item.GetComponent<LevelSelectObj>().Init(i, infos[i].nodeType.ToString());

        }
    }
}
