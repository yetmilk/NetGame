using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFlag : MonoBehaviour
{
    public CampType CampType;

    public List<CampType> friendList = new List<CampType>();

    public void Init(CampType campType)
    {
        CampType = campType;
        friendList.Add(campType);
    }

    public bool CheckIsFriendly(CampType beChecker)
    {
        if (friendList.Contains(beChecker)) return true;
        return false;
    }
}

public enum CampType
{
    None,
    Player,
    Enemy,
}
