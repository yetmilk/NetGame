using Map;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public BlueprintObj levelInfo;




    public virtual void Init(BlueprintObj levelInfo)
    {
        this.levelInfo = levelInfo;

    }


}
