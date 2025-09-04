using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    public string selfId;

    public List<string> activeplayerNameList = new List<string>();
    public List<PlayerInfo> curPlayerInfos = new List<PlayerInfo>();
    public Dictionary<string, PlayerInfo> curActivePlayer = new Dictionary<string, PlayerInfo>();

    public Transform playerInstantiatePos;

    public Action onPlayerCreate;

    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public string netID;
        public CharacterClacify character;
        public CharacterDataObj characterData;
        public CharacterController playerObj;
    }

    public event Action OnPlayerInit;
    protected void Start()
    {

        DontDestroyOnLoad(gameObject);

        RoomManager.Instance.OnCreateRoomComplete += UpdatePlayerList;
        RoomManager.Instance.OnRoomUpdate += UpdatePlayerList;

        //StartCoroutine(UpdatePlayer());
        NetManager.AddMsgListener("MsgUpdatePlayerClacify", UpdateCharacterFromServer);
        NetManager.AddMsgListener("MsgGetPlayerInfo", GetPlayerInfoFromServer);
    }

    //IEnumerator UpdatePlayer()
    //{
    //    while (true)
    //    {
    //        UpdatePlayerList();
    //        yield return new WaitForSeconds(.5f);
    //    }
    //}

    public void UpdatePlayerList()
    {

        foreach (var member in RoomManager.Instance.curRoom.roomMembers)
        {
            if (!activeplayerNameList.Contains(member))
            {
                InitPlayer(member);

            }

        }

        //var list = new List<PlayerInfo>(curActivePlayer.Values.ToList());
        //foreach (var member in list)
        //{
        //    if (!RoomManager.Instance.curRoom.roomMembers.Contains(member.name))
        //    {
        //        DeletePlayer(member.name);
        //    }
        //}


    }

    public void InitPlayer(string playerName, string netID = "", CharacterClacify clacify = CharacterClacify.ою©м)
    {
        if (activeplayerNameList.Contains(playerName)) return;
        activeplayerNameList.Add(playerName);

        PlayerInfo info = new PlayerInfo();
        info.name = playerName;
        info.character = clacify;

        if (string.IsNullOrEmpty(netID))
        {
            MsgGetPlayerInfo msg = new MsgGetPlayerInfo();
            msg.name = playerName;
            NetManager.Send(msg);
        }
        else
        {
            info.netID = netID;
        }
        var dataModule = LoadManager.Instance.GetResourceByName<CharacterDataSO>(info.character.ToString());
        info.characterData = new CharacterDataObj(dataModule);
        curPlayerInfos.Add(info);
        curActivePlayer.Add(playerName, info);
        OnPlayerInit?.Invoke();
    }

    public GameObject CreatePlayer(string playerId)
    {

        var obj = LoadManager.Instance.GetResourceByName<GameObject>(curActivePlayer[playerId].character.ToString());
        var player = Instantiate(obj);
        player.transform.position = playerInstantiatePos.position;

        player.GetComponent<CampFlag>().Init(CampType.Player);

        curActivePlayer[playerId].playerObj = player.GetComponent<CharacterController>();

        if (playerId == selfId)
        {
            CameraManager.Instance.Init(curActivePlayer[playerId].playerObj.transform);
            player.GetComponent<NetMonobehavior>().ClientInit(curActivePlayer[playerId].netID, "Local");
        }
        else
        {
            player.GetComponent<NetMonobehavior>().ClientInit(curActivePlayer[playerId].netID, "Remote");
        }
        onPlayerCreate?.Invoke();
        return player;

    }

    public void DeletePlayer(string playerId)
    {
        if (curActivePlayer.ContainsKey(playerId))
        {
            Destroy(curActivePlayer[playerId].playerObj.gameObject);
            curPlayerInfos.Remove(GetPlayerInfoByName(playerId));
            curActivePlayer.Remove(playerId);
            activeplayerNameList.Remove(playerId);

        }
    }

    public void ClearAllPlayer()
    {
        curActivePlayer.Clear();
        curPlayerInfos.Clear();
        activeplayerNameList.Clear();
    }

    public PlayerInfo GetPlayerInfoByNetId(string netID)
    {
        foreach (var item in curActivePlayer.Values)
        {

            if (item.netID == netID)
                return item;
        }

        return null;
    }
    public PlayerInfo GetPlayerInfoByName(string name)
    {
        if (curActivePlayer.ContainsKey(name))
        {
            return curActivePlayer[name];
        }

        return null;
    }

    public void ChangePlayerClacify(string name, CharacterClacify clacify)
    {

        MsgUpdatePlayerClacify msg = new MsgUpdatePlayerClacify();
        msg.name = name;
        msg.character = clacify.ToString();
        msg.questIp = NetManager.LocalEndPoint;

        NetManager.Send(msg);
    }

    private void UpdateCharacterFromServer(MsgBase msgBase)
    {
        MsgUpdatePlayerClacify msg = (MsgUpdatePlayerClacify)msgBase;


        if (curActivePlayer.ContainsKey(msg.name))
        {
            curActivePlayer[msg.name].character = Enum.Parse<CharacterClacify>(msg.character);

            if (msg.questIp != NetManager.LocalEndPoint) return;
            if (curActivePlayer[msg.name].playerObj != null)
                curActivePlayer[msg.name].playerObj.NetDestroy(curActivePlayer[msg.name].netID,
                    curActivePlayer[msg.name].playerObj.gameObject);
            CreatePlayer(msg.name);
        }


    }
    private void GetPlayerInfoFromServer(MsgBase msgBase)
    {
        MsgGetPlayerInfo msg = msgBase as MsgGetPlayerInfo;

        curActivePlayer[msg.name].netID = msg.netID;
        curActivePlayer[msg.name].character = Enum.Parse<CharacterClacify>(msg.character);
    }
}
