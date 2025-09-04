
using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("模板数据")]
    [SerializeField] private AgentDoMain doMainModule;
    [NonSerialized] public WorldState state;
    public PrimitiveTask currentTask;
    public bool isPlanning;
    public float replanInterval = 5f;
    public bool autoReplan = true;
    public GameObject SensorObj;

    private Planner planner = new Planner();
    private Plan currentPlan;
    private List<ISensor> sensors = new List<ISensor>();
    private float lastPlanTime = 0f;

    void Start()
    {
        // 初始化世界状态
        // doMain.state.StateChanged += OnWorldStateChanged;
        state = doMainModule.state.Clone();

        state.ReplanNeeded += OnReplanNeeded;

        // 初始化所有传感器
        InitializeSensors();


    }

    void Update()
    {
        // 更新所有传感器
        foreach (var sensor in sensors)
        {
            sensor.UpdateSensor();
        }

        // 自动重新规划
        if (autoReplan && Time.time - lastPlanTime > replanInterval)
        {
            Plan();
        }
        if (currentPlan == null|| currentTask == null)
        {
            //Debug.Log("当前plan是否为空:" + currentPlan == null);
            Plan();
        }

    }

    // 规划并执行
    public void Plan()
    {
        if (isPlanning) return;
        Debug.Log(7777);
        isPlanning = true;
        lastPlanTime = Time.time;

        // 生成计划
        var newPlan = planner.CreatePlan(state, doMainModule.rootTasks);

        

        StopAllCoroutines();

        currentPlan = newPlan;
        if (currentPlan.IsValid)
        {

        }
        else
        {
            Debug.LogWarning("No valid plan found");
            currentPlan = null; // 无效计划置空
        }

        isPlanning = false;

        Execute(currentPlan);
    }

    public void Execute(Plan planToExecute)
    {

        // 校验计划有效性
        if (planToExecute == null || !planToExecute.IsValid)
        {
            //Debug.LogError("Cannot execute: Invalid or null plan");
            return;
        }
        // 启动执行协程
        StartCoroutine(planToExecute.Execute(this));
    }

    // 注册传感器
    public void RegisterSensor(ISensor sensor)
    {
        sensors.Add(sensor);
        sensor.Initialize(this);
    }

    // 初始化所有传感器
    private void InitializeSensors()
    {
        // 获取附加到同一GameObject上的所有ISensor组件
        var sensorComponents = SensorObj.GetComponents<ISensor>();
        foreach (var sensor in sensorComponents)
        {
            RegisterSensor(sensor);
        }

    }



    // 重新规划请求回调
    private void OnReplanNeeded(object sender, string key)
    {
        Debug.Log($"Replan triggered by state change: {key}");
        if (!isPlanning)
        {
            Plan();
        }
    }

    private void OnDestroy()
    {
        state.ReplanNeeded -= OnReplanNeeded;
    }


}