using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "角色数据/CharaData")]
public class CharacterDataSO : ScriptableObject
{
    [Header("角色生命值")]
    public int maxhealth = 100;

    [Header("攻击力")]
    public int attackValue;

    [Header("法强")]
    public int skillValue;

    [Header("角色移动速度")]
    public int moveSpeed;//角色移动速度

    [Header("角色闪避速度")]
    public int parrySpeed;

    [Header("物抗")]
    public int normalDefense;

    [Header("法抗")]
    public int skillDefense;


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
    生命上限 = 0,
    移动速度,
    冲刺速度,
    攻击力,
    法强,
    物抗,
    法抗,
    生命值
}

[System.Serializable]
public class CharacterDataObj
{

    [Header("最大生命值")]
    public int maxhealth;
    [Header("当前生命值")]
    public float curHealth;

    [Header("攻击力")]
    public int attackValue;

    [Header("法强")]
    public int skillValue;

    [Header("角色移动速度")]
    public int moveSpeed;//角色移动速度

    [Header("角色闪避速度")]
    public int parrySpeed;

    [Header("物抗")]
    public int normalDefense;

    [Header("法抗")]
    public int skillDefense;


    public CharacterDataObj(CharacterDataSO charaData)
    {
        curHealth = charaData.maxhealth;
        maxhealth = charaData.maxhealth;
        attackValue = charaData.attackValue;
        skillValue = charaData.skillValue;
        moveSpeed = charaData.moveSpeed;
        parrySpeed = charaData.parrySpeed;
        normalDefense = charaData.normalDefense;
        skillDefense = charaData.skillDefense;
    }

    public CharacterDataObj(CharacterDataObj obj)
    {
        curHealth = obj.curHealth;
        maxhealth = obj.maxhealth;
        attackValue = obj.attackValue;
        skillValue = obj.skillValue;
        moveSpeed = obj.moveSpeed;
        parrySpeed = obj.parrySpeed;
        normalDefense = obj.normalDefense;
        skillDefense = obj.skillDefense;
    }

    public int GetDataByEnum(CharaDataEnum enumValue)
    {
        switch (enumValue)
        {
            case CharaDataEnum.生命上限:
                return maxhealth;
            case CharaDataEnum.移动速度:
                return moveSpeed;
            case CharaDataEnum.冲刺速度:
                return parrySpeed;
            case CharaDataEnum.攻击力:
                return attackValue;
            case CharaDataEnum.法强:
                return skillValue;
            case CharaDataEnum.物抗:
                return normalDefense;
            case CharaDataEnum.法抗:
                return skillDefense;
            default:
                return -1;
        }
    }

    public void SetDataByEnum(CharaDataEnum enumValue, int value)
    {
        switch (enumValue)
        {
            case CharaDataEnum.生命上限:
                maxhealth = value;
                break;
            case CharaDataEnum.移动速度:
                moveSpeed = value;
                break;
            case CharaDataEnum.冲刺速度:
                parrySpeed = value;
                break;
            case CharaDataEnum.攻击力:
                attackValue = value;
                break;
            case CharaDataEnum.法强:
                skillValue = value;
                break;
            case CharaDataEnum.物抗:
                normalDefense = value;
                break;
            case CharaDataEnum.法抗:
                skillDefense = value;
                break;
            default:
                break;
        }
    }
}
