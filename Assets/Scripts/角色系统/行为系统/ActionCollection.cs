using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionCollection", menuName = "配置文件/行为配置文件/ActionCollection")]
public class ActionCollection : ScriptableObject
{
    public CharacterClacify characterActionClacify;

    public ActionClacify actionClacify;

    public List<ActionInfo> ActionInfos = new List<ActionInfo>();
}
