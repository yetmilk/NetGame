using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CanBeCancelTag
{
    public Vector2[] canBeCancelRange;

    public ActionTag canCancelTag;

    [Header("����Ϊ��ϵ����ȼ�")]
    public int proprity = 0;

    [Header("��϶�������ʼ�ٷֱ�")]
    public float nextActionStartPercent = 0;


}
