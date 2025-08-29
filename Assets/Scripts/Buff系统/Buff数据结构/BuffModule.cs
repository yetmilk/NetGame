using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "buffModule", menuName = "Buff����/buffModule")]
public class BuffModule : ScriptableObject
{

    public BuffName buffName;

    public BuffTag tag;//buff�ı�ǩ

    [Header("buff�����ȼ�")]
    public int priority;

    [Header("buffִ�м��(��֡Ϊ��λ��60֡/��)")]
    public int tickTime;

    [Header("buff��ֵ��¼�б�")]
    public List<PropMod> propModList;

    [Header("buff�޸�״̬��¼�б�")]
    public List<StateMod> stateModList;


    public OnOccur onOccur;

    public OnRemove onRemove;

    public OnTimeTick onTimeTick;

    public OnBeHurt onBeHurt;

    public OnHurt onHurt;

    public OnBeKillled onBeKillled;

    public OnKill onKill;

}
