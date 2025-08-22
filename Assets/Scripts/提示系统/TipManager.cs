using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipManager : Singleton<TipManager>
{
    [System.Serializable]
    public class Tip
    {
        public TipType Type;
        public GameObject tipObj;
        public TMP_Text tipText;
        // 用于跟踪当前的显示协程
        [HideInInspector] public Coroutine currentCoroutine;
        // 标记是否为手动关闭模式
        [HideInInspector] public bool isManualCloseMode;
    }

    public List<Tip> tipList;
    public Dictionary<TipType, Tip> tipDictionary;

    protected override void Awake()
    {
        base.Awake();
        // 初始化字典
        tipDictionary = new Dictionary<TipType, Tip>();

        foreach (var tip in tipList)
        {
            if (!tipDictionary.ContainsKey(tip.Type))
            {
                tipDictionary.Add(tip.Type, tip);
                // 初始隐藏所有提示
                tip.tipObj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="tipType">提示类型</param>
    /// <param name="content">内容</param>
    /// <param name="showTime">显示时间（-1表示手动关闭模式）</param>
    /// <param name="onCompleted">完成回调</param>
    public void ShowTip(TipType tipType, string content, Action onCompleted = null, float showTime = -1)
    {
        if (tipDictionary.TryGetValue(tipType, out Tip tip))
        {
            // 停止当前正在运行的协程
            if (tip.currentCoroutine != null)
            {
                StopCoroutine(tip.currentCoroutine);
            }

            // 更新提示内容并显示
            tip.tipText.text = content;
            tip.tipObj.SetActive(true);
            tip.tipObj.GetComponent<ITip>().Show();
            tip.isManualCloseMode = showTime < 0;

            // 根据显示时间决定模式
            if (showTime < 0)
            {
                // 手动关闭模式，不启动自动关闭协程
                Debug.Log($"提示 [{tipType}] 已显示（手动关闭模式）");
            }
            else
            {
                // 自动关闭模式
                tip.currentCoroutine = StartCoroutine(ShowTipCor(tip, showTime, onCompleted));
            }
        }
        else
        {
            Debug.LogError($"未找到类型为 {tipType} 的提示配置");
        }
    }

    /// <summary>
    /// 手动关闭指定类型的提示
    /// </summary>
    /// <param name="tipType">提示类型</param>
    /// <param name="invokeCallback">是否触发完成回调</param>
    public void CloseTip(TipType tipType, bool invokeCallback = true)
    {
        if (tipDictionary.TryGetValue(tipType, out Tip tip) && tip.tipObj.activeSelf)
        {
            // 停止协程
            if (tip.currentCoroutine != null)
            {
                StopCoroutine(tip.currentCoroutine);
                tip.currentCoroutine = null;
            }

            // 触发回调
            if (invokeCallback)
            {
                // 如果需要更复杂的回调管理，可以在这里维护一个回调字典
                Debug.Log($"提示 [{tipType}] 已手动关闭");
            }

            // 隐藏提示
            tip.tipText.text = "";
            tip.tipObj.SetActive(false);
            tip.isManualCloseMode = false;
        }
    }

    /// <summary>
    /// 关闭所有提示
    /// </summary>
    public void CloseAllTips()
    {
        foreach (var tip in tipDictionary.Values)
        {
            if (tip.tipObj.activeSelf)
            {
                if (tip.currentCoroutine != null)
                {
                    StopCoroutine(tip.currentCoroutine);
                    tip.currentCoroutine = null;
                }
                tip.tipText.text = "";
                tip.tipObj.SetActive(false);
                tip.isManualCloseMode = false;
            }
        }
    }

    // 自动关闭的协程
    private IEnumerator ShowTipCor(Tip tip, float showTime, Action onCompleted)
    {
        yield return new WaitForSeconds(showTime);

        // 执行完成回调
        onCompleted?.Invoke();

        // 清理并隐藏
        tip.tipText.text = "";
        tip.tipObj.GetComponent<ITip>().Close();
        tip.currentCoroutine = null;
        tip.isManualCloseMode = false;
    }
}

public enum TipType
{
    LogTip,
    JoinRoomQuestTip,//加入房间申请的提示栏
    UpTip,

}
