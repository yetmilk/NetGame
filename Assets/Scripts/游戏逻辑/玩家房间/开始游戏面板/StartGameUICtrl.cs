using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUICtrl : MonoBehaviour
{
    public Button startGameBtn;
    public TMP_Text curplayerText;
    public TMP_Text roomMemberText;

    public int curPlayerNum = 0;
    public int roomMemberNum = 0;

    private void Start()
    {
        startGameBtn.onClick.AddListener(OnStartGAmeBtnClick);
        OnRoomUpdate();
        RoomManager.Instance.OnRoomUpdate += OnRoomUpdate;
    }

    private void Update()
    {
        if (curPlayerNum == roomMemberNum && PlayerManager.Instance.selfId == RoomManager.Instance.curRoom.hostId)
        {
            startGameBtn.interactable = true;
        }
        else
            startGameBtn.interactable = false;
    }

    private void OnRoomUpdate()
    {
        roomMemberNum = RoomManager.Instance.curRoom.roomMembers.Count;
        roomMemberText.text = roomMemberNum.ToString();
    }


    public void SetCurPlayerNum(int num)
    {
        curPlayerNum = num;
        curplayerText.text = curPlayerNum.ToString();
    }

    private void OnStartGAmeBtnClick()
    {
        BattleManager.Instance.StarGame();
    }
}
