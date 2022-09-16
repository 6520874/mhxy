using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public bool enterCurrentRound;//是否进入当前回合
    //public bool isPlayerTurn;//当前回合的阶段（是玩家阶段，还是敌人阶段）
    public bool isPerformingLogic;//正在执行当前阶段的逻辑
    public CharacterFightAI playerAI;
    //public CharacterFightAI enemyAI;
    public ActCode currentActCode;
    public List<SkillObj> skillInfoList;
    public CharacterFightAI playerPrefab;
    public CharacterFightAI enemyPrefab;
    public Transform playerInitPosTrans;
    public Transform[] enemyInitPosTrans;
    public Transform[] playerdieStartMovePath;
    public Transform[] playerdieEndMovePath;
    public Transform[] enemydieStartMovePath;
    public Transform[] enemydieEndMovePath;

    public List<CharacterFightAI> enemyAIList;//当前所有敌人的引用
    public List<CharacterFightAI> currentActAIsList;//当前战斗中需要行动的敌人
    private int currentActIndex;//当前回合行动的人物索引
    public CharacterFightAI currentAI;//当前回合行动的人物
    public CharacterFightAI targetAI;//当前回合想要作用的目标人物
    public CharacterFightAI playerTargetAI;
    public List<CharacterFightAI> playerTargetAIList;

    public int dieCount;//当前阶段死亡的敌人数量

    public GameObject[] skillPrefabGos;
    void Awake()
    {
        Instance = this;
        skillInfoList = new List<SkillObj>()
        {
         new SkillObj()
        {
            ID=0,
            name="横扫千军",
            skillType = SkillType.MELEE,
            delayHitTime = 1.667f,
            decreateMPValue=100,
            soundPath="HengSaoQianJun"
        },
          new SkillObj()
        {
            ID=1,
            name="唧唧歪歪",
            skillType = SkillType.REMOTEAOE,
            delayHitTime = 1f,
            decreateMPValue=35,
            soundPath="JiJiWaiWai"
        },
          new SkillObj()
        {
            ID=2,
            name="水攻",
            skillType = SkillType.REMOTESINGLE,
            delayHitTime = 1f,
            decreateMPValue=65,
            soundPath="ShuiGong"
        }
        };
        enemyAIList = new List<CharacterFightAI>();
        currentActAIsList = new List<CharacterFightAI>();
        playerTargetAIList = new List<CharacterFightAI>();
    }

    private void OnEnable()
    {
        currentActIndex = 0;
        enterCurrentRound = isPerformingLogic = false;
        CreateCharacters();
    }

    void Update()
    {
        if (enterCurrentRound)//回合
        {
            if (!isPerformingLogic)
            {
                if (currentActIndex>=currentActAIsList.Count)
                {
                    //所有人物在当前回合都已行动
                    currentActIndex = 0;
                    UIManager.Instance.ShowOrHidePlayerOrderPanel(true);
                    playerAI.DestoryDebuff();
                    enterCurrentRound = false;
                    return;
                }
                isPerformingLogic = true;
                currentAI = currentActAIsList[currentActIndex];
                //执行当前阶段敌人逻辑 
                if (!currentAI.isPlayer)
                {
                    RandomEnemyAct();
                }
                else
                {
                    targetAI = playerTargetAI;
                }
                currentAI.PerformLogic();
                currentActIndex++;
            }           
        }
    }
    /// <summary>
    /// 设置玩家行为码
    /// </summary>
    public void SetCharacterActCode(ActCode ac,object obj=null)
    {
        playerAI.SetActCodeAndObjValue(ac,obj);
    }
    /// <summary>
    /// 在战斗开始后生成战斗敌人和玩家
    /// </summary>
    public void CreateCharacters()
    {
        playerAI=Instantiate(playerPrefab,playerInitPosTrans);
        playerAI.transform.localPosition = Vector3.zero;
        playerAI.actRate = 8;
        currentActAIsList.Add(playerAI);
        int num = Random.Range(1,10);
        for (int i = 0; i < num; i++)
        {
            enemyAIList.Add(Instantiate(enemyPrefab, enemyInitPosTrans[i]));
            enemyAIList[i].transform.localPosition = Vector3.zero;
            enemyAIList[i].actRate = Random.Range(1,10);
            currentActAIsList.Add(enemyAIList[i]);
            enemyAIList[i].name = i.ToString();
        }
        currentActAIsList= currentActAIsList.OrderByDescending(p => p.actRate).ToList();
    }

    private void OnDisable()
    {        
        if (playerAI)
        {
            Destroy(playerAI.gameObject);
        }
        for (int i = 0; i < enemyAIList.Count; i++)
        {
            if (enemyAIList[i])
            {
                Destroy(enemyAIList[i].gameObject);
            }
        }
        enemyAIList.Clear();
        currentActAIsList.Clear();
    }
    /// <summary>
    /// 移除当前对象并判断战斗是否结束
    /// </summary>
    /// <param name="ci"></param>
    /// <returns></returns>
    public bool ExitFight(CharacterFightAI ci)
    {
        currentActAIsList.Remove(ci);      
        if (ci.isPlayer)
        {
            return true;
        }
        else
        {
            enemyAIList.Remove(ci);
            return currentActAIsList.Count <= 1;
        }
    }

    private void RandomEnemyAct()
    {
        targetAI = playerAI;
        ActCode ac = (ActCode)Random.Range(0, 4);
        object obj = null;
        switch (ac)
        {
            case ActCode.ATTACK:
                obj = playerAI.attackPosTrans.position;
                break;
            case ActCode.DEFEND:
                break;
            case ActCode.SKILL:
                obj = skillInfoList[2];
                break;
            case ActCode.USEITEM:
                obj = 0;
                break;
            default:
                break;
        }
        currentAI.SetActCodeAndObjValue(ac,obj);
    }
    /// <summary>
    /// 使用范围技能造成伤害的目标群体
    /// </summary>
    /// <param name="num">作用个数</param>
    public void GetAttackTargets(int num)
    {
        playerTargetAIList.Clear();
        int targetIndex= enemyAIList.IndexOf(targetAI);
        //当前敌人总数小于技能作用目标数
        if (num>=enemyAIList.Count)
        {
            num = enemyAIList.Count;
        }
        //当前指定目标索引小于数组长度
        if (targetIndex<enemyAIList.Count)
        {
            //指定敌人往后数N个目标后大于或等于作用群体目标数量
            if (enemyAIList.Count-targetIndex>=num)
            {
               playerTargetAIList=enemyAIList.GetRange(targetIndex,num);
            }
            else
            {
                //分两次
                //向前取的数量
                int leftNum = num - (enemyAIList.Count - targetIndex);
                //向后取
                playerTargetAIList.AddRange(enemyAIList.GetRange(targetIndex, num - leftNum));
                //向前取
                playerTargetAIList.AddRange(enemyAIList.GetRange(targetIndex-leftNum,leftNum));
            }
        }
        //当前选定目标正好是最后一个元素
        else
        {
            playerTargetAIList= enemyAIList.GetRange(targetIndex-num,num);
        }
    }

    public void ToNextCharacterAct()
    {
        if (dieCount>0)
        {
            dieCount--;
        }
        if (dieCount<=0)
        {
            isPerformingLogic = false;
            enterCurrentRound = true;
            dieCount = 0;
        }
    }
}
