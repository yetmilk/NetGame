using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public enum SceneName
{
    主菜单, 玩家房间, 地牢
}
public class GameSceneManager : Singleton<GameSceneManager>
{
    public string curSceneName;

    public GameObject ScreenFade;
    public Animator fadeAnim;
    private Action onComplete;

    private void Start()
    {
        NetManager.AddMsgListener("MsgTransScene", LoadSceneFromServer);

    }

    public void LoadSceneToServer(SceneName sceneName, Action onComplete = null)
    {
        FadeIn();
        MsgTransScene msg = new MsgTransScene();

        msg.sceneName = "Scene_" + sceneName.ToString();

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
            FadeOut();
        };
    }

    public void FadeIn()
    {
        fadeAnim.CrossFade("In", 0);
    }
    public void FadeOut()
    {
        fadeAnim.CrossFade("Out", 0);
    }
}
