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
        PlayerManager.Instance.playerInstantiatePos = transform;
        StartCoroutine(CheckPlayer());
    }

    IEnumerator CheckPlayer()
    {
        while (true)
        {
            UpdateOtherPlayer();
            yield return new WaitForSeconds(3f);
        }
    }
    private void UpdateOtherPlayer()
    {
        foreach (var item in PlayerManager.Instance.curPlayerInfos)
        {
            if (item.playerObj == null && !string.IsNullOrEmpty(item.netID))
            {
                var go = PlayerManager.Instance.CreatePlayer(item.name);
            }
        }
    }

    private void OnDestroy()
    {

    }
}
