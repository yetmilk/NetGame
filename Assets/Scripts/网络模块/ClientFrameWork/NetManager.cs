using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    //定义套接字
    static Socket socket;

    //接收缓冲区
    static ByteArray readBuff;

    //写入队列(发送队列)
    static Queue<ByteArray> writeQueue;

    //事件委托类型
    public delegate void EventListener(string err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //消息委托类型
    public delegate void MsgListener(MsgBase msgBase);
    //消息监听队列
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    //消息列表
    static List<MsgBase> msgList = new List<MsgBase>();
    //消息列表长度
    static int msgCount = 0;
    //每一次Update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 60;

    //是否启用心跳
    public static bool isUsePing = true;
    //心跳间隔时间
    public static int pingInterval = 10;
    //上一次发送Ping的时间
    static float lastPingTime = 0;
    //上一次收到Pong的时间
    static float lastPongTime = 0;



    static bool isConnecting = false;
    static bool isClosing = false;

    public static string LocalEndPoint => socket.LocalEndPoint.ToString();

    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();

        //是否正在连接
        isConnecting = false;

        //是否正在关闭
        isClosing = false;

        //消息列表
        msgList = new List<MsgBase>();
        //消息列表长度
        msgCount = 0;

        //上一次发送Ping的时间
        lastPingTime = 0;
        //上一次收到Pong的时间
        lastPongTime = 0;

        //监听PONG协议
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }



    #region-----------------事件操作-------------------
    //添加监听事件
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        else
        {
            eventListeners.Add(netEvent, listener);
        }
    }

    //移除事件监听
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;

            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    //分发事件
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent].Invoke(err);
        }
    }
    #endregion

    #region--------------消息操作--------------------------
    /// <summary>
    /// 添加监听消息
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="listener"></param>
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加事件
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        else
        {
            msgListeners.Add(msgName, listener);
        }
    }

    /// <summary>
    /// 移除消息监听
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="listener"></param>
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;

            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }

    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgBase"></param>
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName].Invoke(msgBase);
        }
    }
    #endregion
    public static void Connect(string ip, int port)
    {
        //状态判断
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect Fail,already Connected");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail,isConnecting");
            return;
        }
        //初始化成员
        InitState();
        //参数设置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);


    }

    //Connect回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;

            socket.EndConnect(ar);

            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "ConnectSucc");
            isConnecting = false;
            Debug.Log(LocalEndPoint);
            //开始接收消息
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect Fail" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, "ConnectFail");
            isConnecting = false;
        }
    }

    //关闭连接
    public static void Close()
    {
        //状态判断
        if (socket == null || !socket.Connected) return;

        if (isConnecting) return;

        //还有数据正在发送
        if (writeQueue.Count > 0) isClosing = true;
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //继续接收数据
            if (readBuff.readIdx < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError("Socket Receive fail" + ex.ToString());
        }
    }

    public static void OnReceiveData()
    {
        //消息长度
        if (readBuff.length <= 2) return;

        //获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength + 2) return;
        readBuff.readIdx += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Debug.Log("OnReceiveDAta MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //添加到消息队列
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCount++;
        //继续获取消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    //发送数据
    public static void Send(MsgBase msg)
    {
        //状态判断
        if (socket == null || !socket.Connected) return;

        if (isConnecting) return;

        if (isClosing) return;

        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //组装消息体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        //写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        //send
        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }

    }

    private static void SendCallback(IAsyncResult ar)
    {
        //获取state、EndSend的处理
        Socket socket = (Socket)ar.AsyncState;

        //状态判断
        if (socket == null || !socket.Connected) return;

        //EndSend
        int count = socket.EndSend(ar);
        //获取写入队列第一条数据
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();

        }
        //完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.FirstOrDefault();
            }
        }

        //继续发送
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        else if (isClosing) socket.Close();
    }

    //Update
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    //更新消息
    private static void MsgUpdate()
    {
        //初步判断，提升效率
        if (msgCount == 0) return;
        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //获取第一条消息
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            }
            //没有消息了
            else break;
        }
    }

    private static void PingUpdate()
    {
        //是否启用
        if (!isUsePing)
        {
            return;
        }
        //发送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        //检测PONG时间
        if (Time.time - lastPingTime > pingInterval * 4)
        {
            Close();
        }
    }

    //监听PONG协议
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}
