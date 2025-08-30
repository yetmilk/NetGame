
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadManager : Singleton<LoadManager>
{

    public Dictionary<string, Dictionary<string, object>> resourceDic = new Dictionary<string, Dictionary<string, object>>();

    protected void Start()
    {

        LoadResourceByLabel<GameObject>("Prefab");
        LoadResourceByLabel<CharacterDataSO>("DataSO");
        LoadResourceByLabel<ActionCollection>("ActionInfo");
        LoadResourceByLabel<ProbabilityConfig>("ProbabilityConfig");
        LoadResourceByLabel<BuffModule>("BuffModule");

        NetManager.AddMsgListener("MsgInstantiateObj", InstantiateFromServer);
    }

    private void LoadResourceByLabel<T>(string label)
    {
        AsyncOperationHandle<IList<T>> op = Addressables.LoadAssetsAsync<T>(label, null);
        op.Completed += (op) =>
        {
            LoadManager_Completed(op, label);
        };
    }

    private void LoadManager_Completed<T>(AsyncOperationHandle<IList<T>> obj, string label)
    {
        if (!resourceDic.ContainsKey(label))
        {
            resourceDic.Add(label, new Dictionary<string, object>());
        }
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var resource in obj.Result)
            {
                if (resource == null) continue;

                // ��ȡ��Դ����
                string resourceName = GetResourceName(resource);

                // �����ظ����ͬ����Դ
                if (!resourceDic[label].ContainsKey(resourceName))
                {
                    resourceDic[label].Add(resourceName, resource);
                }
                else
                {
                    //Debug.LogWarning($"��Դ�� {packageName} �д���ͬ����Դ: {resourceName}���Ѻ����ظ���");
                }
            }
        }
    }
    /// <summary>
    /// ��ȡ��Դ������
    /// </summary>
    public static string GetResourceName(object resource)
    {
        // ��Բ�ͬ���͵���Դ��ȡ����
        if (resource is UnityEngine.Object unityObj)
        {
            return unityObj.name;
        }

        // �����������ͣ�ʹ����ToString()����������
        return resource?.ToString() ?? "UnknownResource";
    }


    public T GetResourceByName<T>(string name)
    {
        // ����������Դ�ֵ�
        foreach (var resource in resourceDic.Values)
        {
            // ����Ƿ����ָ�����Ƶ���Դ
            if (resource.TryGetValue(name, out var value))
            {
                // �����Դ�����Ƿ���Tƥ�䣨�����������ͣ�
                if (value is T tValue)
                {
                    return tValue; // ����ƥ�䣬����ת�����ֵ
                }
                // ���Ͳ�ƥ��
                //Debug.LogWarning($"��Դ {name} ���ڣ������Ͳ�ƥ�䣨����: {typeof(T).Name}��ʵ��: {value.GetType().Name}��");
            }
        }
        // δ�ҵ�ƥ�����Դ
        Debug.LogWarning($"δ�ҵ�����Ϊ {name} ������Ϊ {typeof(T).Name} ����Դ");
        return default;
    }
    public List<T> GetResourcesByLabel<T>(string label)
    {
        List<T> reslult = new List<T>();
        if (resourceDic.ContainsKey(label))
        {
            foreach (var item in resourceDic[label].Values)
            {
                if ((item is T tvalue))
                {
                    reslult.Add(tvalue);
                }
            }
        }

        return reslult;
    }

    public GameObject NetInstantiate(string resourceName, string netId = "")
    {
        var insObj = LoadManager.Instance.GetResourceByName<GameObject>(resourceName);

        if (insObj != null)
        {

            var go = Instantiate(insObj);

            string netID;
            if (string.IsNullOrEmpty(netId))
            {
                Guid guid = Guid.NewGuid();
                netID = guid.ToString();
            }
            else
            {
                netID = netId;
            }
            go.GetComponent<NetMonobehavior>().ClientInit(netID, "Local");

            MsgInstantiateObj msg = new MsgInstantiateObj();
            msg.netId = netID;
            msg.questIp = NetManager.LocalEndPoint.ToString();
            msg.parentNetId = "";
            msg.insObjName = insObj.name;

            NetManager.Send(msg);

            return go;
        }

        return null;
    }

    public GameObject NetInstantiate(string resourceName, Transform transform, string parNetId, bool usePar = false, string netId = "")
    {
        var insObj = LoadManager.Instance.GetResourceByName<GameObject>(resourceName);

        if (insObj != null)
        {

            var go = Instantiate(insObj, transform);

            string netID;
            if (string.IsNullOrEmpty(netId))
            {
                Guid guid = Guid.NewGuid();
                netID = guid.ToString();
            }
            else
            {
                netID = netId;
            }

            go.GetComponent<NetMonobehavior>().ClientInit(netID, "Local");
            MsgInstantiateObj msg = new MsgInstantiateObj();
            msg.netId = netID;
            msg.questIp = NetManager.LocalEndPoint.ToString();
            msg.parentNetId = parNetId;
            msg.insObjName = insObj.name;
            msg.useParent = usePar;

            NetManager.Send(msg);

            return go;
        }

        return null;
    }


    private void InstantiateFromServer(MsgBase msgBase)
    {
        MsgInstantiateObj msg = msgBase as MsgInstantiateObj;

        if (msg.questIp == NetManager.LocalEndPoint.ToString()) return;
        GameObject obj = LoadManager.Instance.GetResourceByName<GameObject>(msg.insObjName);
        obj.GetComponent<NetMonobehavior>().ClientInit(msg.netId, "Remote");
        if (msg.useParent)
        {
            if(msg.parentNetId!="")
            {
                var parentObj = GameNetManager.Instance.GetNetObject(msg.parentNetId);
                var go = Instantiate(obj, parentObj.transform);
            }
           

        }
        else
        {
            if (msg.parentNetId != "")
            {
                var parentObj = GameNetManager.Instance.GetNetObject(msg.parentNetId);
                var go = Instantiate(obj, parentObj.transform);
                go.transform.parent = null;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NetManager.RemoveMsgListener("MsgInstantiateObj", InstantiateFromServer);
    }
}

