using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class JoinRoomQuestTip : MonoBehaviour, ITip
{
    public Animator anim;

    public Button accaptBtn;
    public Button refuseBtn;

    private void Start()
    {
        accaptBtn.onClick.AddListener(() =>
        {
            RoomManager.Instance.ReplayJoinQuest(true);
            TipManager.Instance.CloseTip(TipType.JoinRoomQuestTip);
        });
        refuseBtn.onClick.AddListener(() =>
        {
            RoomManager.Instance.ReplayJoinQuest(false);
            TipManager.Instance.CloseTip(TipType.JoinRoomQuestTip);
        });
    }
    public void Close()
    {
        anim.CrossFade("A_加入房间请求_Close", 0);
        StartCoroutine(WaitCloseEnd());
    }

    public void Show()
    {
        anim.CrossFade("A_加入房间请求_Show", 0);

    }
    IEnumerator WaitCloseEnd()
    {
        yield return new WaitForSeconds(.5f);
        gameObject.SetActive(false);
    }
}
