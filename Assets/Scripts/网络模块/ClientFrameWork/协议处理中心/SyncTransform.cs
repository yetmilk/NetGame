using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    NetMonobehavior target;

    // 同步间隔：固定时间间隔发送，不管是否有变化
    [Header("同步设置")]
    [Tooltip("同步发送间隔（单位：秒），数值越大发送频率越低")]
    public float syncInterval = 1.0f;   // 降低发送频率，默认1秒一次

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
            // 初始时立即同步一次位置和旋转
            SendTransform();

            // 启动固定间隔发送协程
            StartCoroutine(SendTransformAtInterval());
        }
    }

    // 固定间隔发送变换信息的协程
    IEnumerator SendTransformAtInterval()
    {
        while (true)
        {
            // 等待设定的同步间隔
            yield return new WaitForSeconds(syncInterval);

            // 只有本地对象才需要发送同步
            if (!target.IsLocal) continue;

            // 发送当前变换信息（无论是否有变化）
            SendTransform();
        }
    }

    // 发送当前位置和旋转
    private void SendTransform()
    {
        // 发送当前位置
        MsgPosition posMsg = new MsgPosition
        {
            NetId = target.NetID,
            questIp = NetManager.LocalEndPoint,
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z
        };
        NetManager.Send(posMsg);

        // 发送当前旋转
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

        // 调试信息
        // Debug.Log($"发送变换同步：位置 {transform.position}，旋转 {transform.rotation}");
    }

    // 接收位置同步并更新
    private void SyncPosition(MsgBase msgBase)
    {
        MsgPosition msg = msgBase as MsgPosition;
        // 只更新非本地、且NetID匹配的对象
        if (msg == null || target.NetID != msg.NetId || target.IsLocal) return;

        target.transform.position = new Vector3(msg.positionX, msg.positionY, msg.positionZ);
    }

    // 接收旋转同步并更新
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
