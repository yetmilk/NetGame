using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterDataController : MonoBehaviour, IDataContainer
{

    private CharacterDataObj dataModule;

    [SerializeField] private CharacterDataObj dataObj;

    public CharacterController controller;

    public BuffController buffController;

    public CharacterDataObj GetDataModule()
    {
        return dataModule;
    }

    public CharacterDataObj GetDataObj()
    {
        return dataObj;
    }

    /// <summary>
    /// 组件的初始化函数
    /// </summary>
    public void Init(CharacterClacify character, CharacterController charaObj)
    {
        controller = charaObj;
        CharacterDataSO dataSO = LoadManager.Instance.GetResourceByName<CharacterDataSO>(character.ToString());
        if (dataSO != default)
        {
            dataModule = new CharacterDataObj(dataSO);
            buffController = new BuffController(this);
            dataObj = new CharacterDataObj(dataModule);

            NetManager.AddMsgListener("MsgUpdateDataObj", UpdateDataFromServer);

            StartCoroutine(UpdateDataCoro());
        }
    }
    IEnumerator UpdateDataCoro()
    {
        while (true)
        {
            MsgUpdateDataObj msg = new MsgUpdateDataObj();
            msg.characterDataObj = dataObj;
            msg.netId = controller.NetID;
            NetManager.Send(msg);
            yield return new WaitForSeconds(.5f);
        }
    }

    public void UpdateDataFromServer(MsgBase msgBase)
    {
        MsgUpdateDataObj msg = (MsgUpdateDataObj)msgBase;

        if (msg.netId == controller.NetID)
        {
            dataObj = msg.characterDataObj;
        }
    }

    public void CaculateDamage(DamageInfo damageInfo)
    {
        List<BuffObj> buffList = buffController.curBuffsList;

        foreach (var buff in buffList)
        {
            buff.OnHurt(controller, null, ref damageInfo);
        }

        dataObj.curHealth -= damageInfo.damageValue;

    }

}

public interface IDataContainer
{
    public CharacterDataObj GetDataModule();
    public CharacterDataObj GetDataObj();

}
