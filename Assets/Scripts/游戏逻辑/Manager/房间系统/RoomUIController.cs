using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour
{
    [Header("房间信息")]
    public TMP_Text hostIdText;
    public TMP_Text roomMemberNumText;

    [Header("房间成员信息")]
    public RoomMemberUI memberUI1;
    public RoomMemberUI memberUI2;
    public RoomMemberUI memberUI3;

    [Header("退出房间按钮")]
    public Button quitRoomBtn;

    [Header("打开/关闭房间面板按钮")]
    public Button openOrClosePanelBtn;
    public Animator animator;
    bool isOpen = false;

    public GameObject panelObj;


    public void SetRoomPanelActive(bool active)
    {
        panelObj.SetActive(active);
    }

    public void SetHostId(string hostId)
    {
        hostIdText.text = hostId;
    }

    public void SetRoomMemberNum(int roomMemberNum)
    {
        roomMemberNumText.text = roomMemberNum.ToString();
    }

    public void SetRoomMemberInfo(int index, string id, bool activeQuit)
    {
        switch (index)
        {
            case 0:
                memberUI1.Init(id, activeQuit);
                break;
            case 1:
                memberUI2.Init(id, activeQuit);
                break;
            case 2:
                memberUI3.Init(id, activeQuit);
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        quitRoomBtn.onClick.AddListener(() =>
        {
            string hostId = RoomManager.Instance.curRoom.hostId;
            string memberId = PlayerManager.Instance.selfId;
            if (hostId == memberId) return;

            MsgQuitRoom msg = new MsgQuitRoom();
            msg.isKick = false;
            msg.hostId = hostId;
            msg.membersId = memberId;

            NetManager.Send(msg);
        });

        openOrClosePanelBtn.onClick.AddListener(() =>
        {
            Debug.Log(1111);
            isOpen = !isOpen;
            if (isOpen)
            {
                animator.CrossFade("A_房间栏_Show", 0);
            }
            else
            {
                animator.CrossFade("A_房间栏_Close", 0);
            }
            openOrClosePanelBtn.transform.Rotate(Vector3.forward, 180f);
        });
    }

    public void ClearAllMemberInfo()
    {
        memberUI1.Close();
        memberUI2.Close();
        memberUI3.Close();
    }

    public void UpdateRoomUI()
    {
        List<string> roomMembers = RoomManager.Instance.curRoom.roomMembers;
        ClearAllMemberInfo();
        
        for (int i = 0; i < roomMembers.Count; i++)
        {
            bool openKick = PlayerManager.Instance.selfId == RoomManager.Instance.curRoom.hostId;
            SetRoomMemberInfo(i, roomMembers[i], openKick);
        }

        SetRoomMemberNum(roomMembers.Count);

    }
}
