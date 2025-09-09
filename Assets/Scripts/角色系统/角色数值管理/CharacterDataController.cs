using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterDataController : MonoBehaviour, IDataContainer
{

    private CharacterDataObj dataModule;

    [SerializeField] private CharacterDataObj dataObj;

    public CharacterState characterState;

    public CharacterController controller;

    public BuffController buffController;

    public Transform uiParent;

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
    public void Init(ref CharacterDataObj dataObj, CharacterController charaObj, bool initUI)
    {
        controller = charaObj;
        characterState = new CharacterState();

        if (dataObj != null)
        {
            buffController = new BuffController(controller);
            this.dataObj = dataObj;

            NetManager.AddMsgListener("MsgUpdateDataObj", UpdateDataFromServer);

            StartCoroutine(UpdateDataCoro());

            if (initUI)
            {
                var go = LoadManager.Instance.GetResourceByName<GameObject>("血条");
                var obj = Instantiate(go, uiParent);
                obj.GetComponent<UIHp>().Init(ref dataObj);
            }

            if (PlayerManager.Instance.GetPlayerInfoByNetId(controller.NetID) != null
                && PlayerManager.Instance.GetPlayerInfoByNetId(controller.NetID).name == PlayerManager.Instance.selfId)
                BattleManager.Instance.FettersManager.SetPlayer(controller);
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
            yield return new WaitForSeconds(1f);
        }
    }
    private void Update()
    {
        UpdateSkillValue();
    }

    public void UpdateSkillValue()
    {
        if (dataObj == null) return;
        dataObj.curEnergyValue += (Time.deltaTime * dataObj.skillResumeSpeed);
        if (dataObj.curEnergyValue > dataObj.energyValue)
            dataObj.curEnergyValue = dataObj.energyValue;
        if (dataObj.skill1ResumeTimer > 0)
            dataObj.skill1ResumeTimer -= Time.deltaTime;
        if (dataObj.skill2ResumeTimer > 0)
            dataObj.skill2ResumeTimer -= Time.deltaTime;
        if (dataObj.skill3ResumeTimer > 0)
            dataObj.skill3ResumeTimer -= Time.deltaTime;
    }

    public void UpdateDataFromServer(MsgBase msgBase)
    {
        MsgUpdateDataObj msg = (MsgUpdateDataObj)msgBase;

        if (msg.netId == controller.NetID && !controller.IsLocal)
        {
            dataObj.UpdateData(msg.characterDataObj);
        }
    }

    public void CaculateDamage(DamageInfo damageInfo)
    {
        if (controller.NetID != damageInfo.targetNetId) return;
        List<BuffObj> buffList = buffController.curBuffsList;

        foreach (var buff in buffList)
        {
            buff.OnHurt(controller, null, ref damageInfo);
        }

        dataObj.curHealth -= damageInfo.damageValue;

    }

    private void OnDestroy()
    {
        buffController.OnDisable();
        NetManager.RemoveMsgListener("MsgUpdateDataObj", UpdateDataFromServer);
    }
}

public interface IDataContainer
{
    public CharacterDataObj GetDataModule();
    public CharacterDataObj GetDataObj();

}
