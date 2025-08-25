using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Interact_StartGame : InteractObjBase
{

    protected override void OnInteract()
    {
        GameSceneManager.Instance.LoadSceneToServer(SceneName.µÿ¿Œ);
    }
}
