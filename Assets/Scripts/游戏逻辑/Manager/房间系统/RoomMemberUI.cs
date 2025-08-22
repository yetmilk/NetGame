using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMemberUI : MonoBehaviour
{
    public TMP_Text id;
    public Image profile;

    public Button quitBtn;

    public GameObject info;

    public void Init(string id, bool activeBtn)
    {
        info.gameObject.SetActive(true);
        this.id.text = id;

        if (activeBtn)
        {
            if (id != RoomManager.Instance.curRoom.hostId)
            {
                quitBtn.gameObject.SetActive(true);
                quitBtn.onClick.RemoveAllListeners();
                quitBtn.onClick.AddListener(() =>
                {
                    MsgQuitRoom msg = new MsgQuitRoom();
                    msg.isKick = true;
                    msg.hostId = RoomManager.Instance.curRoom.hostId;
                    msg.membersId = id;

                    NetManager.Send(msg);
                });
            }

        }
        else
        {
            quitBtn.gameObject.SetActive(false);
        }

    }

    public void Close()
    {
        info?.gameObject.SetActive(false);
        quitBtn?.gameObject.SetActive(false);
    }
}
