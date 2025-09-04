using Map;
using RectEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public FettersManager FettersManager;
    public MapManager MapManager;
    public UIManager uiManager;
    public void StarGame()
    {
        MapManager.SetRoom(0);

        foreach (var item in PlayerManager.Instance.curPlayerInfos)
        {
            var data = LoadManager.Instance.GetResourceByName<CharacterDataSO>(item.character.ToString());
            if (data != null)
            {
                item.characterData = new CharacterDataObj(data);
            }
        }
    }

    public void GotoNextLevel()
    {
        MapManager.GoToNextLevel();
    }
}
