using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Interact_StartGame : InteractObjBase
{
    protected override void OnInteract()
    {
        LoadManager.Instance.FadeIn();
        Addressables.LoadSceneAsync("Scene_����").Completed += (obj) =>
        {
            if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                LoadManager.Instance.FadeOut();
            }
        };
    }
}
