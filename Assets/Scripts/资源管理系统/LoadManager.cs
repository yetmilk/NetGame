
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

                // 获取资源名称
                string resourceName = GetResourceName(resource);

                // 避免重复添加同名资源
                if (!resourceDic[label].ContainsKey(resourceName))
                {
                    resourceDic[label].Add(resourceName, resource);
                }
                else
                {
                    //Debug.LogWarning($"资源包 {packageName} 中存在同名资源: {resourceName}，已忽略重复项");
                }
            }
        }
    }
    /// <summary>
    /// 获取资源的名称
    /// </summary>
    public static string GetResourceName(object resource)
    {
        // 针对不同类型的资源获取名称
        if (resource is UnityEngine.Object unityObj)
        {
            return unityObj.name;
        }

        // 对于其他类型，使用其ToString()或类型名称
        return resource?.ToString() ?? "UnknownResource";
    }


    public T GetResourceByName<T>(string name)
    {
        // 遍历所有资源字典
        foreach (var resource in resourceDic.Values)
        {
            // 检查是否包含指定名称的资源
            if (resource.TryGetValue(name, out var value))
            {
                // 检查资源类型是否与T匹配（包括派生类型）
                if (value is T tValue)
                {
                    return tValue; // 类型匹配，返回转换后的值
                }
                // 类型不匹配
                //Debug.LogWarning($"资源 {name} 存在，但类型不匹配（期望: {typeof(T).Name}，实际: {value.GetType().Name}）");
            }
        }
        // 未找到匹配的资源
        Debug.LogWarning($"未找到名称为 {name} 且类型为 {typeof(T).Name} 的资源");
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

