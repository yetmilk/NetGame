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
    public Button learnLevelBtn;


    public int curPlayerNum = 0;
    public int roomMemberNum = 0;

    private void Start()
    {
        startGameBtn = transform.GetChild(0).GetComponent<Button>();
        learnLevelBtn = transform.GetChild(1).GetComponent<Button>();
        startGameBtn.onClick.AddListener(OnStartGAmeBtnClick);
        learnLevelBtn.onClick.AddListener(OnLearnBtnClicked);
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

    public void OnStartGAmeBtnClick()
    {
        BattleManager.Instance.StarGame();
    }

    private void OnLearnBtnClicked()
    {
        if (RoomManager.Instance.curRoom.roomMembers.Count == 1)
        {
            GameSceneManager.Instance.LoadSceneLocal(SceneName.初始_教学_火);
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "房间有其他玩家，无法进入教学关卡", null, 3f);
        }
    }
}
