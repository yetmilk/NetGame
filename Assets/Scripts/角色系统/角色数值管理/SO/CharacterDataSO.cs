using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "角色数据/CharaData")]
public class CharacterDataSO : ScriptableObject
{
    [Header("角色生命值")]
    public float maxhealth = 100;

    [Header("攻击力")]
    public int attackValue;

    [Header("法强")]
    public int skillAttackValue;

    [Header("角色移动速度")]
    public int moveSpeed;//角色移动速度

    [Header("角色闪避速度")]
    public int parrySpeed;

    [Header("物抗")]
    public int normalDefense;

    [Header("法抗")]
    public int skillDefense;

    [Header("总精力值")]
    public float skillValue;

    [Header("冲刺消耗精力值")]
    public float parrySpendValue;

    [Header("精力恢复速度(单位/秒)")]
    public float skillResumeSpeed;

    [Header("技能1冷却时间")]
    public float skill1ResumeTime;

    [Header("技能2冷却时间")]
    public float skill2ResumeTime;

    [Header("技能3冷却时间")]
    public float skill3ResumeTime;


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
    生命值,
    精力值,
}

[System.Serializable]
public class CharacterDataObj
{

    [Header("最大生命值")]
    public float maxhealth;
    [Header("当前生命值")]
    public float curHealth;

    [Header("攻击力")]
    public int attackValue;

    [Header("法强")]
    public int skillAttackValue;

    [Header("角色移动速度")]
    public int moveSpeed;//角色移动速度

    [Header("角色闪避速度")]
    public int parrySpeed;

    [Header("物抗")]
    public int normalDefense;

    [Header("法抗")]
    public int skillDefense;


    [Header("总精力值")]
    public float energyValue;

    [Header("当前精力值")]
    public float curEnergyValue;

    [Header("冲刺消耗精力值")]
    public float parrySpendValue;

    [Header("精力恢复速度(单位/秒)")]
    public float skillResumeSpeed;

    [Header("技能1冷却时间")]
    public float skill1ResumeTime;

    [Header("技能1冷却计时器")]
    public float skill1ResumeTimer =0;

    [Header("技能2冷却时间")]
    public float skill2ResumeTime;

    [Header("技能2冷却计时器")]
    public float skill2ResumeTimer =0;

    [Header("技能3冷却时间")]
    public float skill3ResumeTime;

    [Header("技能3冷却计时器")]
    public float skill3ResumeTimer = 0;


    public CharacterDataObj(CharacterDataSO charaData)
    {
        curHealth = charaData.maxhealth;
        maxhealth = charaData.maxhealth;
        attackValue = charaData.attackValue;
        energyValue = charaData.skillValue;
        curEnergyValue = energyValue;
        moveSpeed = charaData.moveSpeed;
        parrySpeed = charaData.parrySpeed;
        normalDefense = charaData.normalDefense;
        skillDefense = charaData.skillDefense;
        skillAttackValue = charaData.skillAttackValue;
        skillResumeSpeed = charaData.skillResumeSpeed;
        parrySpendValue = charaData.parrySpendValue;
        skill1ResumeTime = charaData.skill1ResumeTime;
        skill2ResumeTime = charaData.skill2ResumeTime;
        skill3ResumeTime = charaData.skill3ResumeTime;

    }

    public CharacterDataObj(CharacterDataObj obj)
    {
        curHealth = obj.curHealth;
        maxhealth = obj.maxhealth;
        attackValue = obj.attackValue;
        energyValue = obj.energyValue;
        moveSpeed = obj.moveSpeed;
        parrySpeed = obj.parrySpeed;
        normalDefense = obj.normalDefense;
        skillDefense = obj.skillDefense;
        skillAttackValue = obj.skillAttackValue;
        skillResumeSpeed = obj.skillResumeSpeed;
        parrySpendValue = obj.parrySpendValue;
        skill1ResumeTime = obj.skill1ResumeTime;
        skill2ResumeTime = obj.skill2ResumeTime;
        skill3ResumeTime = obj.skill3ResumeTime;
    }

    public void UpdateData(CharacterDataObj obj)
    {
        if (obj == null) return;
        curHealth = obj.curHealth;
        maxhealth = obj.maxhealth;
        attackValue = obj.attackValue;
        energyValue = obj.energyValue;
        moveSpeed = obj.moveSpeed;
        parrySpeed = obj.parrySpeed;
        normalDefense = obj.normalDefense;
        skillDefense = obj.skillDefense;
        skillAttackValue = obj.skillAttackValue;
        skillResumeSpeed = obj.skillResumeSpeed;
        parrySpendValue = obj.parrySpendValue;
        skill1ResumeTime = obj.skill1ResumeTime;
        skill2ResumeTime = obj.skill2ResumeTime;
        skill3ResumeTime = obj.skill3ResumeTime;
    }
    public float GetDataByEnum(CharaDataEnum enumValue)
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
                return skillAttackValue;
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
                energyValue = value;
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
