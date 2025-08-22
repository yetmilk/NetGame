

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{

    public static ChatManager instance;
    //聊天窗口列表
    public Dictionary<string, ChatLog> logidToLogsDic = new Dictionary<string, ChatLog>();
    public List<string> idList = new List<string>();

    public ChatLog curChatLog;
    private int curLogIndex;

    [Header("UI")]
    //聊天标题
    public Transform logTitleParent;
    public GameObject logTitleObj;
    public Button leftRollBtn;
    public Button rightRollBtn;

    [Space(10)]
    //聊天内容
    public Transform logContentParent;
    public GameObject contentTextObj;
    [Space(10)]
    //输入框
    public TMP_InputField inputField;

    [Space(10)]
    public PlayerInputAction inputs;
    public GameObject ChatUI;


    private void Awake()
    {
        //scrollRect.onValueChanged.AddListener(GetRoll);
        DontDestroyOnLoad(this);
        if (instance == null) instance = this;
        else Destroy(gameObject);

        contentTextObj = LoadManager.Instance.GetResourceByName<GameObject>("聊天内容");
        logTitleObj = LoadManager.Instance.GetResourceByName<GameObject>("聊天标题按钮");

    }

    private void Start()
    {


        NetManager.AddMsgListener("MsgUpdateChatContent", ReceiveText);
        NetManager.AddMsgListener("MsgInitChatLog", InitLogFromServer);

        leftRollBtn.onClick.AddListener(ChangeLeftLog);
        rightRollBtn.onClick.AddListener(ChangeRightLog);
        inputs = new PlayerInputAction();
        inputs.UI.OpenChat.started += OpenChat_started1;
        inputs.Enable();


        Invoke("UpdatateRoomLog", .5f);
    }
    private void UpdatateRoomLog()
    {
        CreateNewLog("队伍", PlayerManager.Instance.activeplayerNameList);
    }

    private void OpenChat_started1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ChatUI.SetActive(!ChatUI.activeSelf);
    }

    #region------------网络回调---------------
    public void InitLogFromServer(MsgBase msgBase)
    {
        MsgInitChatLog msg = msgBase as MsgInitChatLog;
        if (logidToLogsDic.ContainsKey(msg.logId)) return;
        ChatLog chatLog = new ChatLog(msg.members.ToList());
        chatLog.logID = msg.logId;
        chatLog.chatContent = msg.contents;
        chatLog.logTitle = msg.logTitle;
        logidToLogsDic.Add(msg.logId, chatLog);
        idList.Add(msg.logId);

        var chatTitle = Instantiate(logTitleObj, logTitleParent);
        chatTitle.GetComponent<ChatText>().SetText(ChatText.TextType.ChatTitle, msg.logTitle);
        chatTitle.GetComponent<TitleFlag>().title = chatLog.logID;
        ChangeLog(chatLog.logID, msg.logTitle);
    }

    public void ReceiveText(MsgBase msgBase)
    {
        MsgUpdateChatContent msg = msgBase as MsgUpdateChatContent;

        if (!string.IsNullOrEmpty(msg.text))
        {
            ChatLog.ChatContent content = new ChatLog.ChatContent();
            content.text = msg.text;
            content.id = msg.sendId;

            logidToLogsDic[msg.logid].chatContent.Add(content);

            if (curChatLog.logID == msg.logid)
            {
                var go = Instantiate(contentTextObj, logContentParent);
                go.GetComponent<ChatText>().SetText(ChatText.TextType.ChatContent, msg.text, msg.sendId);
            }
        }
    }

    #endregion
    #region-------网络消息发送---------
    public void CreateNewLog(string logTitle, List<string> members)
    {
        MsgInitChatLog msg = new MsgInitChatLog();
        msg.members = members.ToArray();
        msg.logTitle = logTitle;
        msg.contents = new List<ChatLog.ChatContent>();

        NetManager.Send(msg);
    }

    public void SendChatText()
    {
        MsgUpdateChatContent msg = new MsgUpdateChatContent();
        msg.text = inputField.text;
        inputField.text = "";
        //inputField.ActivateInputField();
        msg.members = curChatLog.chatLogMembers.ToArray();
        msg.logid = curChatLog.logID;
        msg.sendId = PlayerManager.Instance.selfId;
        NetManager.Send(msg);
    }
    #endregion

    public void ChangeLog(string logName, string logtitle)
    {

        string logid = logName;

        curChatLog = logidToLogsDic[logid];
        curLogIndex = idList.IndexOf(logid);

        if (!idList.Contains(logid))
        {
            Destroy(logTitleParent.GetChild(0).gameObject);
            var chatTitle = Instantiate(logTitleObj, logTitleParent);
            idList.Add(logid);
            chatTitle.GetComponent<ChatText>().SetText(ChatText.TextType.ChatTitle, logtitle);

        }
        else
        {
            var chatTitle = Instantiate(logTitleObj, logTitleParent);
            chatTitle.GetComponent<ChatText>().SetText(ChatText.TextType.ChatTitle, logtitle);

        }

        for (int i = logContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(logContentParent.GetChild(i).gameObject);
        }
        if (curChatLog.chatContent == null || curChatLog.chatContent.Count == 0) return;
        foreach (var item in curChatLog.chatContent)
        {
            var go = Instantiate(contentTextObj, logContentParent);
            go.GetComponent<ChatText>().SetText(ChatText.TextType.ChatContent, item.text, item.id);
        }
    }

    #region ------------------按钮调用--------------------
    public void ChangeLeftLog()
    {
        if (curLogIndex - 1 >= 0)
        {
            string nextid = idList[curLogIndex - 1];
            string nextTitle = logidToLogsDic[nextid].logTitle;
            ChangeLog(nextid, nextTitle);
        }
    }

    public void ChangeRightLog()
    {
        if (curLogIndex + 1 < idList.Count)
        {
            string nextid = idList[curLogIndex + 1];
            string nextTitle = logidToLogsDic[nextid].logTitle;
            ChangeLog(nextid, nextTitle);
        }
    }

    public void OpenChatLog()
    {
        ChatUI.SetActive(true);

    }

    #endregion
    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgUpdateChatContent", ReceiveText);
        NetManager.RemoveMsgListener("MsgInitChatLog", InitLogFromServer);
    }


}

[System.Serializable]
public class ChatLog
{
    public string logTitle = "";
    public string logID = "";
    public List<string> chatLogMembers = new List<string>();

    public List<ChatContent> chatContent;

    [System.Serializable]
    public struct ChatContent
    {
        public string id;
        public string text;

    }

    public ChatLog(List<string> chatLogMembers)
    {
        this.chatLogMembers = chatLogMembers;
        chatContent = new List<ChatContent>();
    }

    /// <summary>
    /// 检查owner与finder组合是否与chatLogMembers中的角色完全匹配（忽略顺序）
    /// </summary>
    /// <param name="owner">所有者角色名</param>
    /// <param name="finder">查找者角色数组</param>
    /// <param name="chatLogMembers">聊天日志成员数组</param>
    /// <returns>如果元素完全匹配返回true，否则返回false</returns>
    public bool IsRoleInChatLogMembers(string[] finder, string[] chatLogMembers)
    {
        string owner = PlayerManager.Instance.selfId;
        // 处理null情况
        if (finder == null)
            finder = Array.Empty<string>();

        // 正确拼接数组：将owner添加到finder数组中
        string[] combinedRoles = new string[finder.Length + 1];
        finder.CopyTo(combinedRoles, 0);
        combinedRoles[finder.Length] = owner;

        // 处理chatLogMembers为null的情况
        if (chatLogMembers == null)
            chatLogMembers = Array.Empty<string>();

        // 长度不同直接返回false
        if (combinedRoles.Length != chatLogMembers.Length)
            return false;

        // 排序后比较每个元素
        var sortedCombined = combinedRoles.OrderBy(s => s).ToArray();
        var sortedMembers = chatLogMembers.OrderBy(s => s).ToArray();

        for (int i = 0; i < sortedCombined.Length; i++)
        {
            // 使用string.Equals更安全，避免null引用异常
            if (!string.Equals(sortedCombined[i], sortedMembers[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

}
