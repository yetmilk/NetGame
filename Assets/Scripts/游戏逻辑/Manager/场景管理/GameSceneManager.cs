using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public enum SceneName
{
    主菜单, 玩家房间, 地牢, 怪物房_火
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
        MsgTransScene msg = new MsgTransScene();

        msg.sceneName = "Scene_" + sceneName.ToString();

        NetManager.Send(msg);

        this.onComplete += onComplete;
    }



    public void LoadSceneToServer(string sceneName, Action onComplete = null)
    {

        MsgTransScene msg = new MsgTransScene();

        msg.sceneName = sceneName.ToString();

        NetManager.Send(msg);

        this.onComplete += onComplete;
    }

    private void LoadSceneFromServer(MsgBase msgBase)
    {
        FadeIn();
        MsgTransScene msg = msgBase as MsgTransScene;

        var op = Addressables.LoadSceneAsync(msg.sceneName);

        op.Completed += (op) =>
        {
            onComplete?.Invoke();
            curSceneName = msg.sceneName;
            FadeOut();
        };
    }

    public void LoadSceneLocal(SceneName sceneName, Action onComplete = null)
    {
        FadeIn();
        string scene = "Scene_" + sceneName.ToString();
        var op = Addressables.LoadSceneAsync(scene);
        op.Completed += (op) =>
        {
            onComplete?.Invoke();
            curSceneName = sceneName.ToString();
            FadeOut();
        };
    }
    public void LoadSceneLocal(string sceneName, Action onComplete = null)
    {
        FadeIn();
        var op = Addressables.LoadSceneAsync(sceneName);
        op.Completed += (op) =>
        {
            onComplete?.Invoke();
            curSceneName = sceneName.ToString();
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
