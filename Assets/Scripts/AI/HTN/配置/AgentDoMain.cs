using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DoMain", menuName = "HTN/DoMain")]
public class AgentDoMain : ScriptableObject
{
    public WorldState state;

    public List<Task> rootTasks;


    public AgentDoMain(AgentDoMain doMain)
    {
        this.state = doMain.state.Clone();
        this.rootTasks = new List<Task>();

        for (int i = 0; i < doMain.rootTasks.Count; i++)
        {
            rootTasks.Add(doMain.rootTasks[i]);
        }
    }
}