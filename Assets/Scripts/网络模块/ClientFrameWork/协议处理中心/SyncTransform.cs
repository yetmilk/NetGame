using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    NetMonobehavior target;

    // ͬ��������̶�ʱ�������ͣ������Ƿ��б仯
    [Header("ͬ������")]
    [Tooltip("ͬ�����ͼ������λ���룩����ֵԽ����Ƶ��Խ��")]
    public float syncInterval = 1.0f;   // ���ͷ���Ƶ�ʣ�Ĭ��1��һ��

    private void Awake()
    {
        NetManager.AddMsgListener("MsgPosition", SyncPosition);
        NetManager.AddMsgListener("MsgRotation", SyncRotation);
        target = GetComponent<NetMonobehavior>();
    }

    private void Start()
    {
        if (target.IsLocal)
        {
            // ��ʼʱ����ͬ��һ��λ�ú���ת
            SendTransform();

            // �����̶��������Э��
            StartCoroutine(SendTransformAtInterval());
        }
    }

    // �̶�������ͱ任��Ϣ��Э��
    IEnumerator SendTransformAtInterval()
    {
        while (true)
        {
            // �ȴ��趨��ͬ�����
            yield return new WaitForSeconds(syncInterval);

            // ֻ�б��ض������Ҫ����ͬ��
            if (!target.IsLocal) continue;

            // ���͵�ǰ�任��Ϣ�������Ƿ��б仯��
            SendTransform();
        }
    }

    // ���͵�ǰλ�ú���ת
    private void SendTransform()
    {
        // ���͵�ǰλ��
        MsgPosition posMsg = new MsgPosition
        {
            NetId = target.NetID,
            questIp = NetManager.LocalEndPoint,
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z
        };
        NetManager.Send(posMsg);

        // ���͵�ǰ��ת
        MsgRotation rotMsg = new MsgRotation
        {
            NetId = target.NetID,
            questIp = NetManager.LocalEndPoint,
            rotationX = transform.rotation.x,
            rotationY = transform.rotation.y,
            rotationZ = transform.rotation.z,
            rotationW = transform.rotation.w
        };
        NetManager.Send(rotMsg);

        // ������Ϣ
        // Debug.Log($"���ͱ任ͬ����λ�� {transform.position}����ת {transform.rotation}");
    }

    // ����λ��ͬ��������
    private void SyncPosition(MsgBase msgBase)
    {
        MsgPosition msg = msgBase as MsgPosition;
        // ֻ���·Ǳ��ء���NetIDƥ��Ķ���
        if (msg == null || target.NetID != msg.NetId || target.IsLocal) return;

        target.transform.position = new Vector3(msg.positionX, msg.positionY, msg.positionZ);
    }

    // ������תͬ��������
    private void SyncRotation(MsgBase msgBase)
    {
        MsgRotation msg = msgBase as MsgRotation;
        if (msg == null || target.NetID != msg.NetId || target.IsLocal) return;

        target.transform.rotation = new Quaternion(msg.rotationX, msg.rotationY, msg.rotationZ, msg.rotationW);
    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgPosition", SyncPosition);
        NetManager.RemoveMsgListener("MsgRotation", SyncRotation);
        StopAllCoroutines();
    }
}
