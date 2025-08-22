using System.Collections.Generic;
using UnityEngine;
using static ChatLog;

public class MsgUpdateChatContent : MsgBase
{
    public MsgUpdateChatContent() { protoName = "MsgUpdateChatContent"; }
    public string text;

    public string sendId;
    public string logid;
    public string[] members;
}

public class MsgInitChatLog : MsgBase
{
    public MsgInitChatLog() { protoName = "MsgInitChatLog"; }

    public string logTitle;
    public string logId;
    public string[] members;
    public List<ChatContent> contents;

}
