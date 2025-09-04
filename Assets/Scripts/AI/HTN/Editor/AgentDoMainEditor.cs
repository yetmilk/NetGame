using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(AgentDoMain))]
public class AgentDoMainEditor : Editor
{
    private AgentDoMain agentDoMain;
    private int selectedTaskTypeIndex = 0;
    private string[] taskTypeOptions = { "Compound Task", "Primitive Task" };

    private void OnEnable()
    {
        agentDoMain = (AgentDoMain)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        // 下拉菜单选择任务类型
        selectedTaskTypeIndex = EditorGUILayout.Popup("Select Task Type", selectedTaskTypeIndex, taskTypeOptions);

        if (GUILayout.Button("Add New Task"))
        {
            switch (selectedTaskTypeIndex)
            {
                case 0: // Compound Task
                    CreateCompoundTask();
                    break;
                case 1: // Primitive Task
                    CreatePrimitiveTask();
                    break;
            }
        }
    }

    private void CreateCompoundTask()
    {
        CompoundTask compoundTask = ScriptableObject.CreateInstance<CompoundTask>();
        compoundTask._state = agentDoMain.state;
        compoundTask.taskName = "New Compound Task";

        string assetPath = GetDomainAssetPath();
        AssetDatabase.CreateAsset(compoundTask, assetPath + "/NewCompoundTask.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        agentDoMain.rootTasks.Add(compoundTask);
        EditorUtility.SetDirty(agentDoMain);
    }

    private void CreatePrimitiveTask()
    {
        PrimitiveTask primitiveTask = ScriptableObject.CreateInstance<PrimitiveTask>();
        primitiveTask.taskName = "New Primitive Task";
        primitiveTask.precondition = null;
        primitiveTask.effects = null;
        primitiveTask.executeAction = null;
        primitiveTask._state = agentDoMain.state;

        string assetPath = GetDomainAssetPath();
        AssetDatabase.CreateAsset(primitiveTask, assetPath + "/NewPrimitiveTask.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        agentDoMain.rootTasks.Add(primitiveTask);
        EditorUtility.SetDirty(agentDoMain);
    }

    private string GetDomainAssetPath()
    {
        string assetPath = AssetDatabase.GetAssetPath(agentDoMain);
        return Path.GetDirectoryName(assetPath);
    }
}