using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    public Button createRoomBtn;
    public Button joinRoomBtn;
    public Button settingBtn;
    public Button quitBtn;

    private void Start()
    {
        createRoomBtn.onClick.AddListener(CreateNewRoom);
        joinRoomBtn.onClick.AddListener(JoinRoomBtn);
        settingBtn.onClick.AddListener(SettingBtn);
        quitBtn.onClick.AddListener(QuitBtn);
    }

    public void CreateNewRoom()
    {
        RoomManager.Instance.CreateRoom(PlayerManager.Instance.selfId, new List<string>() { PlayerManager.Instance.selfId });
    }

    public void JoinRoomBtn()
    {

    }

    public void SettingBtn()
    {

    }

    public void QuitBtn()
    {

    }
}
