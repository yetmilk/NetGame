using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Interact_StartGame : InteractObjBase
{
    public StartGameUICtrl uICtrl;

    // 存储当前在触发器范围内的碰撞体
    private HashSet<Collider> _insideColliders = new HashSet<Collider>();

    // 当前范围内的碰撞体数量
    public int CurrentCount => _insideColliders.Count;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            if (other.GetComponent<CharacterController>().IsLocal)
                CameraManager.Instance.SetCameraRotation(Quaternion.Euler(new Vector3(60, 0, 0)));

            // 防止重复添加（例如同一个物体有多个碰撞体的情况）
            if (!_insideColliders.Contains(other))
            {
                _insideColliders.Add(other);
                uICtrl.SetCurPlayerNum(CurrentCount);
                //Debug.Log($"碰撞体进入: {other.name}，当前数量: {CurrentCount}");
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            if (other.GetComponent<CharacterController>().IsLocal)
            {
                CameraManager.Instance.SetCameraRotation(Quaternion.Euler(new Vector3(45, 0, 0)));
                uICtrl.gameObject.SetActive(false);
            }


            if (_insideColliders.Contains(other))
            {
                _insideColliders.Remove(other);
                uICtrl.SetCurPlayerNum(CurrentCount);

            }
        }
    }


    protected override void OnInteract()
    {
        uICtrl.gameObject.SetActive(true);
    }
}
