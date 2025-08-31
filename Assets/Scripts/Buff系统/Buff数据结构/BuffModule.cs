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

    [Header("�Ƿ������ô��ڵ�buff")]
    public bool isForever;

    [Header("buff����ʱ�䣨�룩")]
    public int lifeTime;


  

}
