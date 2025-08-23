using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerTestEditor : MonoBehaviour
{
    [Header("是否是调试模式")]
    public bool isTest;

    [Header("要测试的角色")]
    public CharacterClacify character;

    private void Start()
    {
        if (isTest)
        {

            //gameObject.AddComponent<FrameManager>();
            CharacterDataSO data = null;
            ActionCollection actionInfos = null;
            var handle = Addressables.LoadAssetAsync<CharacterDataSO>(character.ToString());
            handle.WaitForCompletion();
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                data = handle.Result;

            var handle1 = Addressables.LoadAssetAsync<ActionCollection>(character.ToString());
            handle1.WaitForCompletion();
            if (handle1.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                actionInfos = handle1.Result;
            Addressables.LoadAssetAsync<GameObject>(character.ToString()).Completed += (obj) =>
            {
                if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var player = Instantiate(obj.Result);
                    player.transform.position = transform.position;
                    //player.AddComponent<CharacterInput>().Init();

                }
            };
        }
    }


}
