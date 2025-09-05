using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLearnLevelController : LevelController
{
    public TextComponent textComponent;
    public Animator targetAnimator;
    public CharacterController target;

    public MonsterSpawner spawner1;
    public MonsterSpawner spawner2;
    public MonsterSpawner spawner3;
    public MonsterSpawner spawner4;

    public Transform box;

    public int Allstep = 4;
    public int curStep = 0;

    private void Start()
    {
        NextStep();
    }

    public void NextStep()
    {
        switch (curStep)
        {
            case 0:
                StartCoroutine(Step1Coro()); break;
            case 1:
                StartCoroutine(Step2Coro()); break;
            case 2:
                StartCoroutine(Step3Coro()); break;
            case 3:
                StartCoroutine(Step4Coro()); break;
            default:
                break;
        }
    }
    WaitForSeconds wait1s = new WaitForSeconds(1);
    WaitForSeconds wait2s = new WaitForSeconds(2);
    WaitForSeconds wait3s = new WaitForSeconds(3);
    WaitForSeconds wait4s = new WaitForSeconds(4);
    bool moveUp, moveDown, moveLeft, moveRight, parry = false;
    public IEnumerator Step1Coro()
    {
        targetAnimator.CrossFade("A_�ؿ�Ŀ��_Ŀ��1", 0);
        textComponent.ShowText("��ӭ������ѧ�ؿ����������㽫�������Ϸ��һ����������ʶ");
        yield return wait4s;
        target = PlayerManager.Instance.GetPlayerInfoByName(PlayerManager.Instance.selfId).playerObj;
        textComponent.ShowText("����������ѧϰ����ƶ���ɫ");
        yield return wait4s;
        textComponent.ShowText("ʹ�ü��̵�WASD�ٿؽ�ɫ���������ƶ�");
        yield return wait4s;
        textComponent.ShowText("����W�������ƶ���");
        Action<ActionTag, ActionObj, ActionObj> actionUp = (a, b, c) =>
        {
            if (a == ActionTag.Move && b.direction == Vector3.forward)
            {
                moveUp = true;
            }
        };
        target.selfActionCtrl.OnActionExit += actionUp;
        while (!moveUp)
        {
            yield return null;
        }
        target.selfActionCtrl.OnActionExit -= actionUp;
        textComponent.ShowText("����S�������ƶ���");
        Action<ActionTag, ActionObj, ActionObj> actionDown = (a, b, c) =>
        {
            if (a == ActionTag.Move && b.direction == -Vector3.forward)
            {
                moveDown = true;
            }
        };
        target.selfActionCtrl.OnActionExit += actionDown;
        while (!moveDown)
        {
            yield return null;
        }
        target.selfActionCtrl.OnActionExit -= actionDown;
        textComponent.ShowText("����A�������ƶ���");
        Action<ActionTag, ActionObj, ActionObj> actionLeft = (a, b, c) =>
        {
            if (a == ActionTag.Move && b.direction == Vector3.left)
            {
                moveLeft = true;
            }
        };
        target.selfActionCtrl.OnActionExit += actionLeft;
        while (!moveLeft)
        {
            yield return null;
        }
        target.selfActionCtrl.OnActionExit -= actionLeft;
        textComponent.ShowText("����D�������ƶ���");
        Action<ActionTag, ActionObj, ActionObj> actionRight = (a, b, c) =>
        {
            if (a == ActionTag.Move && b.direction == Vector3.right)
            {
                moveRight = true;
            }
        };
        target.selfActionCtrl.OnActionExit += actionRight;
        while (!moveRight)
        {
            yield return null;
        }
        target.selfActionCtrl.OnActionExit -= actionRight;

        textComponent.ShowText("�����ƶ����⣬��ɫ�����԰��¿ո���г�̣����¿ո���һ�γ�̰�");

        Action<ActionTag, ActionObj, ActionObj> actionParry = (a, b, c) =>
        {
            if (a == ActionTag.Parry)
            {
                parry = true;
            }
        };
        target.selfActionCtrl.OnActionExit += actionParry;
        while (!parry)
        {
            yield return null;
        }
        target.selfActionCtrl.OnActionExit -= actionParry;
        textComponent.ShowText("ʹ�ó�̻����Ľ�ɫ������ֵ������ֵ�ڽ�ɫ��ͷ���鿴��Ҫע�Ᵽ����������Ŷ");
        yield return wait4s;

        textComponent.ShowText("�������Ѿ�ѧ���˽�ɫ�Ļ���������������������ѧϰ���Ӹ��ӵĲ���");
        yield return wait4s;
        curStep++;
        NextStep();
    }

    public IEnumerator Step2Coro()
    {
        targetAnimator.CrossFade("A_�ؿ�Ŀ��_Ŀ��2", 0);
        textComponent.ShowText("�����ǵ���Ϸ�У�ս���Ƿǳ���Ҫ��һ��");
        yield return wait4s;
        textComponent.ShowText("���ǵĽ�ɫ�����ֹ�����ʽ��һ������ͨ��������һ���ǽ�ɫ����");
        yield return wait3s;
        textComponent.ShowText("����������ѧϰ��ͨ�������������������Խ�����ͨ����");
        yield return wait4s;
        textComponent.ShowText("�����ɫ�����Լ����ص��������ܣ��ֱ�ʹ��Q����E������Shift������");
        yield return wait4s;
        textComponent.ShowText("����ʹ����ͨ�����ͼ��ܴ����ǰ��ֻ�����");
        yield return wait4s;
        textComponent.Close();
        var monsters = spawner1.SpawnMonsters(1);
        CharacterController monster = monsters[0].GetComponent<CharacterController>();
        bool monsterDead = false;
        monster.selfActionCtrl.OnActionEnter += (a, b) =>
        {
            if (a == ActionTag.Dead)
            {
                monsterDead = true;
            }
        };
        while (!monsterDead)
        {
            yield return null;
        }
        yield return wait2s;
        Destroy(monster.gameObject);
        textComponent.ShowText("�������Ѿ�ѧ�����������˽���ս��", true);
        yield return wait4s;
        curStep++;
        NextStep();


    }

    public IEnumerator Step3Coro()
    {
        targetAnimator.CrossFade("A_�ؿ�Ŀ��_Ŀ��3", 0);
        textComponent.ShowText("��ս���Ĺ����У�����Ҳ�Ƕ�ս��Ӱ��ܴ��һ�������õ��Σ���ʱ�ܴ������벻����Ч��");
        yield return wait4s;
        textComponent.ShowText("���ΰ���������ҽ��͹ؿ��ڴ��ڵ����壬��Щ��Ʒ���Զ�������ǹ�������˺�");
        yield return wait4s;
        textComponent.ShowText("�ҽ���Ч����������ȥ���κ���������������ע����ҽ����־���");
        yield return wait4s;
        var monsters = spawner2.SpawnMonsters(1);
        bool monsterDead = false;
        CharacterController monster = monsters[0].GetComponent<CharacterController>();
        monster.selfActionCtrl.OnActionEnter += (a, b) =>
        {
            if (a == ActionTag.Dead)
            {
                monsterDead = true;
            }
        };
        while (!monsterDead)
        {
            yield return null;
        }
        yield return wait2s;
        Destroy(monster.gameObject);
        textComponent.ShowText("�����ļһ���ҽ����������");
        yield return wait4s;
        curStep++;
        NextStep();
    }

    public IEnumerator Step4Coro()
    {
        targetAnimator.CrossFade("A_�ؿ�Ŀ��_Ŀ��4", 0);
        textComponent.ShowText("�ڽ�ÿ���ؿ��Ĺ���������Ϻ��㽫���ñ�������ص��ؼ�");
        box.Rotate(new Vector3(-120, 0, 0));

        BattleManager.Instance.FettersManager.AddRareBook(RareBookName.��ˮ��);
        yield return wait4s;
        textComponent.ShowText("�����ͨ������Tab���鿴��ǰ�����е��ؼ����ٴΰ���Tab�ر����");
        yield return wait4s;
        textComponent.ShowText("���ڣ���һ�����������ս����");
        yield return wait4s;
        textComponent.Close();
        var monster1 = spawner1.SpawnMonsters(2);

        var monster2 = spawner3.SpawnMonsters(2);

        monster1.AddRange(monster2);
        while (true)
        {
            bool monsterDead = true;
            foreach (var item in monster1)
            {
                if (item.GetComponent<CharacterController>().selfActionCtrl.curActionObj.curActionInfo.tag != ActionTag.Dead)
                    monsterDead = false;
            }
            if (monsterDead)
            {
                break;
            }
            yield return null;
        }

        textComponent.ShowText("���ν�ѧ�ؿ������������Ѿ�����������������ҷ���");

        yield return wait4s;
        GameSceneManager.Instance.LoadSceneLocal(SceneName.��ҷ���);

    }
}
