using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "角色数据/CharaData")]
public class CharacterDataSO : ScriptableObject
{
    [Header("角色生命值")]
    public int maxhealth = 100;


    [Header("角色移动速度")]
    public int moveSpeed;//角色移动速度

    [Header("角色闪避速度")]
    public int parrySpeed;


    public CharacterDataSO(CharacterDataSO characterData)
    {
        moveSpeed = characterData.moveSpeed;
        parrySpeed = characterData.parrySpeed;
    }

    public CharacterDataSO()
    {

    }
}

public enum CharaDataEnum
{
    角色最大生命 = 0,
    角色移动速度,
    角色冲刺速度,
}

[System.Serializable]
public class CharacterDataObj
{
    public CharacterDataSO charaData;

    public float curHealth;

    public Vector3 faceDiraction = Vector3.right;//角色面朝方向

    public Vector3 moveDir;//角色移动方向

    public float rotationSpeed = 100f;

    public CharacterDataObj(CharacterDataSO charaData)
    {
        this.charaData = ScriptableObjectCloner.Clone<CharacterDataSO>(charaData);
        curHealth = charaData.maxhealth;
    }

    public int GetDataByEnum(CharaDataEnum enumValue)
    {
        switch (enumValue)
        {
            case CharaDataEnum.角色最大生命:
                return charaData.maxhealth;
            case CharaDataEnum.角色移动速度:
                return charaData.moveSpeed;
            case CharaDataEnum.角色冲刺速度:
                return charaData.parrySpeed;
            default:
                return -1;
        }
    }
}
