using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "buffModule", menuName = "Buff配置/buffModule")]
public class BuffModule : ScriptableObject
{

    public BuffName buffName;

    public BuffTag tag;//buff的标签

    [Header("buff的优先级")]
    public int priority;

    [Header("buff执行间隔(以帧为单位，60帧/秒)")]
    public int tickTime;

    [Header("buff数值记录列表")]
    public List<PropMod> propModList;

    [Header("buff修改状态纪录列表")]
    public List<StateMod> stateModList;

    [Header("是否是永久存在的buff")]
    public bool isForever;

    [Header("buff持续时间（秒）")]
    public int lifeTime;


  

}
