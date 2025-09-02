using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public FettersManager FettersManager;
    public MapManager MapManager;
    public void StarGame()
    {
        MapManager.SetRoom(0);


    }

    public void GotoNextLevel()
    {
        MapManager.GoToNextLevel();
    }
}
