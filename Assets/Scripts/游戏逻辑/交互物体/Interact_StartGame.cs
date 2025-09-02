using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Interact_StartGame : InteractObjBase
{
    public StartGameUICtrl uICtrl;

    // �洢��ǰ�ڴ�������Χ�ڵ���ײ��
    private HashSet<Collider> _insideColliders = new HashSet<Collider>();

    // ��ǰ��Χ�ڵ���ײ������
    public int CurrentCount => _insideColliders.Count;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            if (other.GetComponent<CharacterController>().IsLocal)
                CameraManager.Instance.SetCameraRotation(Quaternion.Euler(new Vector3(60, 0, 0)));

            // ��ֹ�ظ���ӣ�����ͬһ�������ж����ײ��������
            if (!_insideColliders.Contains(other))
            {
                _insideColliders.Add(other);
                uICtrl.SetCurPlayerNum(CurrentCount);
                //Debug.Log($"��ײ�����: {other.name}����ǰ����: {CurrentCount}");
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
