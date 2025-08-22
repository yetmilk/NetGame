using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstantiate : MonoBehaviour
{
    public Transform InsPostion;
    private void Awake()
    {

    }
    private void Start()
    {
        PlayerManager.Instance.playerInstantiatePos = transform; ;
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        Debug.Log(666);
        var go = PlayerManager.Instance.CreatePlayer(PlayerManager.Instance.selfId);
        CameraManager.Instance.Init(go.transform);
    }

    private void OnDestroy()
    {

    }
}
