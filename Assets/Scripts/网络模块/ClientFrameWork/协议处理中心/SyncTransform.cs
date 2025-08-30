using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SyncTransform : MonoBehaviour
{
    NetMonobehavior target;
    private void Awake()
    {
        NetManager.AddMsgListener("MsgPosition", SyncPosition);
        NetManager.AddMsgListener("MsgRotation", SyncRotation);
        target = GetComponent<NetMonobehavior>();


    }
    private void Start()
    {
       
        if (target.IsLocal)
            StartCoroutine(UpdataeTransform());
    }
    IEnumerator UpdataeTransform()
    {
        while (true)
        {

            MsgPosition msgPosition = new MsgPosition();
            msgPosition.NetId = target.NetID;
            msgPosition.questIp = NetManager.LocalEndPoint;
            msgPosition.positionX = transform.position.x;
            msgPosition.positionY = transform.position.y;
            msgPosition.positionZ = transform.position.z;
            NetManager.Send(msgPosition);

            MsgRotation msgRotation = new MsgRotation();
            msgRotation.NetId = target.NetID;
            msgRotation.questIp = NetManager.LocalEndPoint;
            msgRotation.rotationX = transform.rotation.x;
            msgRotation.rotationY = transform.rotation.y;
            msgRotation.rotationZ = transform.rotation.z;
            msgRotation.rotationW = transform.rotation.w;
            NetManager.Send(msgRotation);

            yield return new WaitForSeconds(.1f);
        }

    }
   
    private void SyncPosition(MsgBase msgBase)
    {
        MsgPosition msg = msgBase as MsgPosition;
        if (target.NetID == msg.NetId  && !target.IsLocal)
        {
            target.transform.position = new Vector3(msg.positionX, msg.positionY, msg.positionZ);
        }
    }

    private void SyncRotation(MsgBase msgBase)
    {
        MsgRotation msg = msgBase as MsgRotation;
        if (target.NetID == msg.NetId && !target.IsLocal)
        {
            Vector3 newFaceDir = new Quaternion(msg.rotationX, msg.rotationY, msg.rotationZ, msg.rotationW).eulerAngles;
            target.transform.rotation = new Quaternion(msg.rotationX, msg.rotationY, msg.rotationZ, msg.rotationW);
        }
    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgPosition", SyncPosition);
        NetManager.RemoveMsgListener("MsgRotation", SyncRotation);
    }

}
