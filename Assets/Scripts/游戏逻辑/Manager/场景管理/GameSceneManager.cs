using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameSceneManager : Singleton<GameSceneManager>
{
    public string curSceneName;

    private Action onComplete;

    private void Start()
    {
        NetManager.AddMsgListener("MsgTransScene", LoadSceneFromServer);

    }

    public void LoadSceneToServer(string sceneName, Action onComplete = null)
    {
        MsgTransScene msg = new MsgTransScene();

        msg.sceneName = sceneName;

        NetManager.Send(msg);

        this.onComplete += onComplete;
    }

    private void LoadSceneFromServer(MsgBase msgBase)
    {
        MsgTransScene msg = msgBase as MsgTransScene;

        var op = Addressables.LoadSceneAsync(msg.sceneName);

        op.Completed += (op) =>
        {
            onComplete?.Invoke();
            curSceneName = msg.sceneName;
        };
    }
}
