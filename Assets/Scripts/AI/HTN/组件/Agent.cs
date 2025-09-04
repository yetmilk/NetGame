
using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("ģ������")]
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
        // ��ʼ������״̬
        // doMain.state.StateChanged += OnWorldStateChanged;
        state = doMainModule.state.Clone();

        state.ReplanNeeded += OnReplanNeeded;

        // ��ʼ�����д�����
        InitializeSensors();


    }

    void Update()
    {
        // �������д�����
        foreach (var sensor in sensors)
        {
            sensor.UpdateSensor();
        }

        // �Զ����¹滮
        if (autoReplan && Time.time - lastPlanTime > replanInterval)
        {
            Plan();
        }
        if (currentPlan == null|| currentTask == null)
        {
            //Debug.Log("��ǰplan�Ƿ�Ϊ��:" + currentPlan == null);
            Plan();
        }

    }

    // �滮��ִ��
    public void Plan()
    {
        if (isPlanning) return;
        Debug.Log(7777);
        isPlanning = true;
        lastPlanTime = Time.time;

        // ���ɼƻ�
        var newPlan = planner.CreatePlan(state, doMainModule.rootTasks);

        

        StopAllCoroutines();

        currentPlan = newPlan;
        if (currentPlan.IsValid)
        {

        }
        else
        {
            Debug.LogWarning("No valid plan found");
            currentPlan = null; // ��Ч�ƻ��ÿ�
        }

        isPlanning = false;

        Execute(currentPlan);
    }

    public void Execute(Plan planToExecute)
    {

        // У��ƻ���Ч��
        if (planToExecute == null || !planToExecute.IsValid)
        {
            //Debug.LogError("Cannot execute: Invalid or null plan");
            return;
        }
        // ����ִ��Э��
        StartCoroutine(planToExecute.Execute(this));
    }

    // ע�ᴫ����
    public void RegisterSensor(ISensor sensor)
    {
        sensors.Add(sensor);
        sensor.Initialize(this);
    }

    // ��ʼ�����д�����
    private void InitializeSensors()
    {
        // ��ȡ���ӵ�ͬһGameObject�ϵ�����ISensor���
        var sensorComponents = SensorObj.GetComponents<ISensor>();
        foreach (var sensor in sensorComponents)
        {
            RegisterSensor(sensor);
        }

    }



    // ���¹滮����ص�
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