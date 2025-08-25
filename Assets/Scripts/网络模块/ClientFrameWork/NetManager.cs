using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public static class NetManager
{
    //定义套接字
    static Socket socket;

    //接收缓冲区
    static ByteArray readBuff;

    //写入队列(发送队列)
    static ConcurrentQueue<ByteArray> writeQueue;

    static readonly object sendLock = new object();
    static bool isSending = false;

    //事件委托类型
    public delegate void EventListener(string err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //消息委托类型
    public delegate void MsgListener(MsgBase msgBase);
    //消息监听队列
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    private static readonly object listenerLock = new object(); // 监听字典专用锁

    // 主线程消息队列（用于存放需要主线程处理的消息）
    static ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();


    static ConcurrentQueue<MsgBase> msgQueue = new ConcurrentQueue<MsgBase>();

    static readonly AutoResetEvent msgEvent = new AutoResetEvent(false);

    //独立的消息分发线程
    static Thread dispatchThread;

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
        writeQueue = new ConcurrentQueue<ByteArray>();

        //是否正在连接
        isConnecting = false;

        //是否正在关闭
        isClosing = false;

        // 重置消息队列
        msgQueue = new ConcurrentQueue<MsgBase>();
        msgEvent.Reset(); // 重置信号

        //上一次发送Ping的时间
        lastPingTime = 0;
        //上一次收到Pong的时间
        lastPongTime = 0;

        //监听PONG协议
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
        if (dispatchThread != null && dispatchThread.IsAlive)
        {
            // 若线程已存在，无需重复创建（可加终止逻辑，视需求而定）
            return;
        }
        dispatchThread = new Thread(DispatchLoop);
        dispatchThread.IsBackground = true; // 后台线程：主程序退出时自动终止
        dispatchThread.Start();
    }

    private static void QueueOnMainThread(Action action)
    {
        if (action == null) return;
        mainThreadActions.Enqueue(action);
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
        if (string.IsNullOrEmpty(msgName) || listener == null) return;

        lock (listenerLock) // 仅锁定字典操作
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            else
            {
                msgListeners.Add(msgName, listener);
            }
        }
    }

    /// <summary>
    /// 移除消息监听
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="listener"></param>
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (string.IsNullOrEmpty(msgName) || listener == null) return;

        lock (listenerLock) // 仅锁定字典操作
        {
            if (msgListeners.TryGetValue(msgName, out var existing))
            {
                existing -= listener;
                if (existing == null)
                {
                    msgListeners.Remove(msgName);
                }
                else
                {
                    msgListeners[msgName] = existing;
                }
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
        MsgListener listener = null;
        lock (listenerLock)
        {
            msgListeners.TryGetValue(msgName, out listener);
        }

        if (listener != null)
        {

            QueueOnMainThread(() =>
            {
                try
                {
                    Debug.Log($"分发消息: {msgName} 实例: {msgBase.GetHashCode()}");
                    listener.Invoke(msgBase);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"消息[{msgName}]回调异常: {ex.Message}");
                }
            });
        }
    }

    private static bool isDispatchThreadRunning = true;
    private static void DispatchLoop()
    {
        while (isDispatchThreadRunning)
        {
            try
            {
                msgEvent.WaitOne(); // 等待消息信号（无消息时阻塞，不占用CPU）
                int processed = 0;
                while (processed < MAX_MESSAGE_FIRE && msgQueue.TryDequeue(out MsgBase msg))
                {
                    FireMsg(msg.protoName, msg);
                    processed++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"消息分发线程异常: {ex.Message}");
            }
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
        if (socket == null || !socket.Connected) return;
        if (isConnecting) return;

        lock (sendLock)
        {
            // 情况1：有数据正在发送或队列有数据，标记为关闭中
            if (isSending || writeQueue.Count > 0)
            {
                isClosing = true;
                Debug.Log("关闭中：等待发送队列清空...");
            }
            // 情况2：无发送任务，直接关闭
            else
            {
                socket.Close();
                FireEvent(NetEvent.Close, "无发送任务，直接关闭");
                isClosing = false;
            }
        }

        // 终止分发线程
        isDispatchThreadRunning = false;
        msgEvent.Set(); // 唤醒阻塞的线程，使其退出循环
        if (dispatchThread != null && dispatchThread.IsAlive)
        {
            dispatchThread.Join(1000); // 等待1秒，确保线程退出
            dispatchThread = null;
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
        readBuff.CheckAndMoveBytes();
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
        if (msgBase != null)
        {
            msgQueue.Enqueue(msgBase);
            msgEvent.Set(); // 触发消息处理信号
        }
        else
        {
            Debug.LogError($"消息 {protoName} 解析失败");
        }
        //继续获取消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    //发送数据
    public static void Send(MsgBase msg)
    {
        // 状态判断（保留原有校验）
        if (socket == null || !socket.Connected) return;
        if (isConnecting) return;
        if (isClosing) return;

        string msgId = $"{DateTime.Now.Ticks}_{msg.protoName}";
        //Debug.Log($"客户端发送消息: {msg.protoName}，唯一标识: {msgId}");

        // 1. 数据编码（原有逻辑保留）
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int totalLen = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + totalLen]; // 2字节长度头 + 消息体

        // 组装长度（小端序：低字节在前，高字节在后）
        sendBytes[0] = (byte)(totalLen % 256);    // 低8位
        sendBytes[1] = (byte)(totalLen / 256);    // 高8位
                                                  // 组装协议名
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        // 组装协议体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        // 2. 加入发送队列（仅这一步，删除直接BeginSend）
        ByteArray ba = new ByteArray(sendBytes);
        writeQueue.Enqueue(ba);

        // 3. 触发发送（双重检查锁，确保仅一个线程启动发送）
        lock (sendLock)
        {
            if (!isSending)
            {
                isSending = true;
                // 从队列取第一条数据发送
                if (writeQueue.TryDequeue(out ByteArray firstBa))
                {
                    // 发送时需指定「有效数据范围」：从readIdx开始，长度为length
                    socket.BeginSend(
                        buffer: firstBa.bytes,
                        offset: firstBa.readIdx,
                        size: firstBa.length,
                        socketFlags: 0,
                        callback: SendCallback,
                        state: socket
                    );
                }
                else
                {
                    // 极端情况：队列刚被清空，重置发送状态
                    isSending = false;
                }
                //重置发送状态
                isSending = false;
            }
        }
    }

private static void SendCallback(IAsyncResult ar)
{
        lock (sendLock) // 确保isSending状态修改线程安全
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                if (socket == null || !socket.Connected)
                {
                    isSending = false;
                    return;
                }

                // 1. 完成本次发送（获取实际发送字节数，用于校验）
                int sentCount = socket.EndSend(ar);
                Debug.Log($"本次发送字节数: {sentCount}");

                // 2. 处理下一条消息（从队列取数据）
                if (writeQueue.TryDequeue(out ByteArray nextBa))
                {
                    // 继续发送下一条
                    socket.BeginSend(
                        buffer: nextBa.bytes,
                        offset: nextBa.readIdx,
                        size: nextBa.length,
                        socketFlags: 0,
                        callback: SendCallback,
                        state: socket
                    );
                }
                else
                {
                    // 队列空了，重置发送状态
                    isSending = false;
                    // 若处于关闭中，此时可安全关闭socket
                    if (isClosing)
                    {
                        socket.Close();
                        FireEvent(NetEvent.Close, "发送队列清空后关闭");
                        isClosing = false; // 重置关闭状态
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError($"发送回调异常: {ex.Message}，错误码: {ex.ErrorCode}");
                isSending = false;
                Close(); // 发送失败，触发关闭
            }
            catch (Exception ex)
            {
                Debug.LogError($"发送回调未知异常: {ex.Message}");
                isSending = false;
            }
        }
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
    // 每帧从主线程队列中取出所有Action并执行（确保在Unity主线程）
    while (mainThreadActions.TryDequeue(out Action action))
    {
        action.Invoke();
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
