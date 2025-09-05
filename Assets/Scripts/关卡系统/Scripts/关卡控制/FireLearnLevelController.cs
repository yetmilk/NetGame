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
        targetAnimator.CrossFade("A_关卡目标_目标1", 0);
        textComponent.ShowText("欢迎来到教学关卡，在这里你将对这个游戏有一个基本的认识");
        yield return wait4s;
        target = PlayerManager.Instance.GetPlayerInfoByName(PlayerManager.Instance.selfId).playerObj;
        textComponent.ShowText("首先我们来学习如何移动角色");
        yield return wait4s;
        textComponent.ShowText("使用键盘的WASD操控角色上下左右移动");
        yield return wait4s;
        textComponent.ShowText("按下W键向上移动吧");
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
        textComponent.ShowText("按下S键向下移动吧");
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
        textComponent.ShowText("按下A键向左移动吧");
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
        textComponent.ShowText("按下D键向左移动吧");
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

        textComponent.ShowText("除了移动以外，角色还可以按下空格进行冲刺，按下空格尝试一次冲刺吧");

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
        textComponent.ShowText("使用冲刺会消耗角色的体力值，体力值在角色的头顶查看，要注意保持体力充沛哦");
        yield return wait4s;

        textComponent.ShowText("现在你已经学会了角色的基本操作，接下来我们来学习更加复杂的操作");
        yield return wait4s;
        curStep++;
        NextStep();
    }

    public IEnumerator Step2Coro()
    {
        targetAnimator.CrossFade("A_关卡目标_目标2", 0);
        textComponent.ShowText("在我们的游戏中，战斗是非常重要的一环");
        yield return wait4s;
        textComponent.ShowText("我们的角色有两种攻击方式，一种是普通攻击，另一种是角色技能");
        yield return wait3s;
        textComponent.ShowText("让我们先来学习普通攻击，按下鼠标左键可以进行普通攻击");
        yield return wait4s;
        textComponent.ShowText("另外角色还有自己独特的三个技能，分别使用Q键，E键和左Shift键触发");
        yield return wait4s;
        textComponent.ShowText("尝试使用普通攻击和技能打败面前这只怪物吧");
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
        textComponent.ShowText("现在你已经学会了如何与敌人进行战斗", true);
        yield return wait4s;
        curStep++;
        NextStep();


    }

    public IEnumerator Step3Coro()
    {
        targetAnimator.CrossFade("A_关卡目标_目标3", 0);
        textComponent.ShowText("在战斗的过程中，地形也是对战斗影响很大的一环，善用地形，有时能带来意想不到的效果");
        yield return wait4s;
        textComponent.ShowText("地形包括这里的岩浆和关卡内存在的陷阱，这些物品可以对你或者是怪物造成伤害");
        yield return wait4s;
        textComponent.ShowText("岩浆的效果会让走上去的任何生物灼烧起来，注意和岩浆保持距离");
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
        textComponent.ShowText("可怜的家伙，被岩浆活活烫死了");
        yield return wait4s;
        curStep++;
        NextStep();
    }

    public IEnumerator Step4Coro()
    {
        targetAnimator.CrossFade("A_关卡目标_目标4", 0);
        textComponent.ShowText("在将每个关卡的怪物清理完毕后，你将会获得宝箱中珍藏的秘籍");
        box.Rotate(new Vector3(-120, 0, 0));

        BattleManager.Instance.FettersManager.AddRareBook(RareBookName.凝水身法);
        yield return wait4s;
        textComponent.ShowText("你可以通过按下Tab键查看当前所持有的秘籍，再次按下Tab关闭面板");
        yield return wait4s;
        textComponent.ShowText("现在，来一场酣畅淋漓的战斗吧");
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

        textComponent.ShowText("本次教学关卡的所有内容已经结束，即将返回玩家房间");

        yield return wait4s;
        GameSceneManager.Instance.LoadSceneLocal(SceneName.玩家房间);

    }
}
