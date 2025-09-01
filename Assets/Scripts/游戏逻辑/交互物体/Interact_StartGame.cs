using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Interact_StartGame : MonoBehaviour
{



    protected void OnInteract()
    {
        MapManager.Instance.SetRoom(0);
    }
}
