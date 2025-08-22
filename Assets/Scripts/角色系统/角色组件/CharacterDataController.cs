using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterDataController : MonoBehaviour, IDataContainer
{

    public CharacterDataObj data;

    public CharacterDataObj GetDataObj()
    {
        return data;
    }

    /// <summary>
    /// 组件的初始化函数
    /// </summary>
    public void Init(CharacterClacify character)
    {

        CharacterDataSO dataSO = LoadManager.Instance.GetResourceByName<CharacterDataSO>(character.ToString());
        if (dataSO != default)
        {
            data = new CharacterDataObj(dataSO);
        }
    }

    public void Init(CharacterDataSO dataSO)
    {

        if (dataSO != default)
        {
            this.data = new CharacterDataObj(dataSO);
        }
    }

}

public interface IDataContainer
{
    public CharacterDataObj GetDataObj();

}
