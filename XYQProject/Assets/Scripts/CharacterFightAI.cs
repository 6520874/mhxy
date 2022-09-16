using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class CharacterFightAI : MonoBehaviour
{
    public NavMeshAgent meshAgent;
    public bool isMoving;
    private Vector3 attackTargetPos;
    public Transform attackPosTrans;
    public CharacterAnimatorController cac;
    public int HP;
    public int currentHP;
    public int MP;
    public int currentMP;
    public bool isPlayer;
    public Vector3 initPos;
    private bool isReturning;
    public bool isDead;
    public int dieMovePointIndex;
    private Vector3 dieMovePoint;
    public Transform defendPosTrans;//闪避，防御，受击
    public GameObject dodgeEffectGo;
    public int attackCount;//攻击次数（技能使用）
    public int currentAttackCount;
    public ActCode actCode;
    public NumCanvas numCanvas;
    public CharacterCanvas characterCanvas;
    public GameObject attackEffectPrefab;
    public GameObject defendEffectPrefab;
    public GameObject recoverHPEffectPrefab;
    public GameObject recoverMPEffectPrefab;
    public int currentUseItemID;
    public GameObject hengSaoDebuffPrefab;
    public SkillObj skillObj;//当前技能参数
    public int actRate;//行动速度
    private object actObjValue;
    public GameObject debuffGo;
    public bool resting;
    public int AOENum;//范围技能伤害目标个数
    public SpriteRenderer spriteRenderer;
    public Color initColor;
    void Start()
    {
        meshAgent.updateUpAxis = false;
        meshAgent.updateRotation = false;
        meshAgent.SetDestination(transform.position);
        initPos = transform.position;
        currentHP = HP;
        if (isPlayer)
        {
            HP = PlayerManager.Instance.currentMaxHP;
            currentHP = PlayerManager.Instance.currentHP;
            MP = PlayerManager.Instance.maxMP;
            currentMP = PlayerManager.Instance.currentMP;
        }
        initColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead)
        {
            if (isMoving)
            {
                DieMove();
            }
            else//非死亡移动 需要设置移动状态
            {
                SetDieMoveState();
            }
            return;
        }
        if (isMoving)
        {
            if (isReturning)
            {
                ReturnBehaviour();
            }
            else
            {
                MoveBehaviour();
            }           
        }
    }

    private void DieMove()
    {
        if (Vector3.Distance(transform.position,dieMovePoint)<=0.2f)
        {
            dieMovePointIndex++;
            isMoving = false;
            if (dieMovePointIndex>=3)
            {
                if (GameController.Instance.ExitFight(this))
                {
                    GameMananger.Instance.EnterOrExitFightMode(false);
                }
                EndCurrentTurn();
                Destroy(gameObject);               
            }
        }
    }

    /// <summary>
    /// 设置死亡移动的状态
    /// </summary>
    private void SetDieMoveState()
    {
        Transform[] dieStartMovePath;
        Transform[] dieEndMovePath;
        if (isPlayer)
        {
            dieStartMovePath = GameController.Instance.playerdieStartMovePath;
            dieEndMovePath = GameController.Instance.playerdieEndMovePath;
        }
        else
        {
            dieStartMovePath = GameController.Instance.enemydieStartMovePath;
            dieEndMovePath = GameController.Instance.enemydieEndMovePath;
        }
        if (dieMovePointIndex<2)//死亡移动
        {
            Vector3 movePoint;
            movePoint=dieStartMovePath[Random.Range(0, dieStartMovePath.Length)].position;
            while (movePoint==dieMovePoint)
            {
                movePoint= dieStartMovePath[Random.Range(0, dieStartMovePath.Length)].position;
            }
            dieMovePoint = movePoint;
        }
        else//即将结束死亡移动
        {
            dieMovePoint = dieEndMovePath[Random.Range(0, dieEndMovePath.Length)].position;
        }
        dieMovePoint.z = transform.position.z;
        meshAgent.SetDestination(dieMovePoint);
        isMoving = true;
    }
    /// <summary>
    /// 设置当前回合的行为码和所需参数
    /// </summary>
    public void SetActCodeAndObjValue(ActCode ac, object obj)
    {
        actCode = ac;
        actObjValue = obj;
    }

    /// <summary>
    /// 执行当前阶段的逻辑
    /// </summary>
    /// <param name="ac">行为指令码</param>
    /// <param name="obj"></param>
    public void PerformLogic()
    {
        if (resting)
        {
            EndCurrentTurn(true);
            resting = false;
            return;
        }
        switch (actCode)
        {
            case ActCode.ATTACK:
                SetMoveAction(true, (Vector3)actObjValue);
                break;
            case ActCode.DEFEND:
                EndCurrentTurn();
                break;
            case ActCode.SKILL:
                skillObj = (SkillObj)actObjValue;       
                if (skillObj.skillType==SkillType.MELEE)
                {
                    ShowMPValueChange(-skillObj.decreateMPValue);
                    attackCount = 3;           
                    SetMoveAction(true, GameController.Instance.targetAI.attackPosTrans.position);
                    AudioSourceManager.Instance.PlaySound(skillObj.soundPath);

                }
                else
                {                   
                    UseItemOrUseRemoteSkillBehaviour();
                }
                break;
            case ActCode.USEITEM:
                currentUseItemID = (int)actObjValue;
                UseItemOrUseRemoteSkillBehaviour();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 释放技能造成的移动事件
    /// </summary>
    public void SetSkillMoveAction()
    {
        currentAttackCount++;       
        SetMoveAction(currentAttackCount<attackCount,transform.position);
    }

    /// <summary>
    /// 设置攻击指令执行前的行为（目标点，是朝目标点移动还是返回初始位置）
    /// </summary>
    /// <param name="moveAct">true进攻，false返回</param>
    /// <param name="attackPos"></param>
    public void SetMoveAction(bool moveAct = false,Vector3 attackPos=default)
    {
        isMoving = true;
        if (moveAct)//进攻
        {
            //设置攻击的目标位置
            attackTargetPos = attackPos;
            meshAgent.SetDestination(attackTargetPos);
            //设置移动动画 todo
            cac.PlayMoveAnimation(-1);   
            isReturning = false;
        }
        else//返回
        {           
            //设置返回位置
            meshAgent.SetDestination(initPos);
            //设置移动动画 todo
            cac.PlayMoveAnimation(1);
            isReturning = true;
        }
        
    }
    /// <summary>
    /// 移动行为(进攻移动)
    /// </summary>
    public void MoveBehaviour()
    {
        if (Vector3.Distance(transform.position,attackTargetPos)<=0.2f)
        {
            isMoving = false;
            //攻击
            AttackBehaviour();
        }
    }
    /// <summary>
    /// 返回行为
    /// </summary>
    public void ReturnBehaviour()
    {
        if (Vector3.Distance(transform.position, initPos) <= 0.2f)
        {
            isMoving = false;
            cac.PlayIdleAnimation();
            //结束自身回合
            EndCurrentTurn();
        }
    }
    /// <summary>
    /// 结束当前自身回合
    /// </summary>
    private void EndCurrentTurn(bool ifResetActCode = false)
    {
        //释放范围技能
        if (actCode==ActCode.SKILL&&skillObj.skillType==SkillType.REMOTEAOE)
        {
            for (int i = 0; i < GameController.Instance.playerTargetAIList.Count; i++)
            {
                if (GameController.Instance.playerTargetAIList[i].isDead)
                {
                    ResetState(ifResetActCode);
                    return;
                }
            }
        }
        else
        {
            //当前目标对象死亡，且死亡的这个目标对象不是当前这个脚本挂载对象（当前是攻击者）
            if (!isDead&&GameController.Instance.targetAI)
            {
                if (GameController.Instance.targetAI != this && GameController.Instance.targetAI.isDead)
                {
                    ResetState(ifResetActCode);
                    return;
                }
            }
        }
        ResetState(ifResetActCode);
        GameController.Instance.ToNextCharacterAct();
    }

    /// <summary>
    /// 攻击行为
    /// </summary>
    public void AttackBehaviour()
    {
        if (isPlayer)
        {
            AudioSourceManager.Instance.PlaySound("PlayerAttack");
        }
        else
        {
            AudioSourceManager.Instance.PlaySound("EnemyAttack");
        }
        cac.PlayAttackAnimation();
    }
    public void AttackTarget()
    {
        if (actCode == ActCode.SKILL)
        {
            ShowHPValueChange(-Random.Range(1, 20));
            if (currentAttackCount == 2)
            {
                resting = true;
                debuffGo = Instantiate(hengSaoDebuffPrefab, transform);
                debuffGo.transform.localPosition = Vector3.zero;
            }
            GameController.Instance.targetAI.HitBehaviour(false);
        }
        else
        {
            GameController.Instance.targetAI.HitBehaviour();
        }
    }

    /// <summary>
    /// 受击行为
    /// </summary>
    public void HitBehaviour(bool canDodge=true,bool canDefend=true)
    {
        meshAgent.isStopped = true;
        //闪避
        if (Random.Range(0,3)>=2&&canDodge)
        {
            //闪避成功
            DodgeBehaviour();
            return;
        }
        //防御
        if (actCode==ActCode.DEFEND && canDefend)
        {
            DefendBehaviour();
            return;
        }
        //受击
        int random = Random.Range(10,150);
        ShowHPValueChange(-random);
        cac.PlayHitAnimation();
        CharacterFightAI ca=GameController.Instance.currentAI;
        if (ca.isPlayer)
        {
            AudioSourceManager.Instance.PlaySound("EnemyHit");
        }
        else
        {
            AudioSourceManager.Instance.PlaySound("PlayerHit");
        }
        if (ca.attackCount> 0)
        {
            //近战技能攻击
            Instantiate(GameController.Instance.skillPrefabGos[ca.skillObj.ID], transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
        else
        {
            if (ca.actCode!=ActCode.SKILL)
            {
                //普通攻击
                Instantiate(attackEffectPrefab, transform.position + new Vector3(0, 0.3f, 0.5f), Quaternion.identity);
            }            
        }        
        JudgeIfDie();
    }
    /// <summary>
    /// 判断是否死亡
    /// </summary>
    private void JudgeIfDie()
    {
        if (currentHP <= 0)
        {
            //死亡
            cac.PlayDieAnimation();
            Time.timeScale = 0.3f;
            transform.DoMove(defendPosTrans.position, 0.2f)
               .SetOnComplete(
                () =>
                {
                    meshAgent.isStopped = false;
                    meshAgent.speed = 15;   
                }
                );
        }
        else
        {
            //受击
            ToDenfendPos(0.2f);
        }
    }
    /// <summary>
    /// 死亡行为
    /// </summary>
    public void DieBehaviour()
    {
        AudioSourceManager.Instance.PlaySound("FlyAway");
        Time.timeScale = 1;
        isDead = true;
        isMoving = false;
        GameController.Instance.enterCurrentRound = false;
        //主要目的是为了放横扫一类的技能没有在原始位置，要回去
        GameController.Instance.currentAI.ResetState();
        GameController.Instance.dieCount++;
    }
    /// <summary>
    /// 闪避行为
    /// </summary>
    public void DodgeBehaviour()
    {
        //添加特效
        dodgeEffectGo.SetActive(true);
        ToDenfendPos(0.15f, () => { dodgeEffectGo.SetActive(false); });
    }
    /// <summary>
    /// 移动到防御位置并返回
    /// </summary>
    /// <param name="animationTime">动画时间</param>
    /// <param name="callBack">需要在动画完成后进行的额外回调</param>
    private void ToDenfendPos(float animationTime,UnityAction callBack=null)
    {
        transform.DoMove(defendPosTrans.position, animationTime)
            .SetOnComplete(
            () =>
            {
                transform.DoMove(initPos, animationTime).SetOnComplete
                (
                    () =>
                    {
                        meshAgent.isStopped = false;
                        if (callBack!=null)
                        {
                            callBack();
                        }                    
                    }          
                );
            }
            );
    }

    /// <summary>
    /// 防御行为
    /// </summary>
    public void DefendBehaviour()
    {
        AudioSourceManager.Instance.PlaySound("PlayerDefend");
        Instantiate(defendEffectPrefab,cac.transform);
        int random = Random.Range(1,50);
        ShowHPValueChange(-random);
        cac.PlayDefendAnimation();
        JudgeIfDie();
    }
    /// <summary>
    /// 重置状态
    /// </summary>
    public void ResetState(bool ifResetActCode=false)
    {
        if (isDead)
        {
            return;
        }
        if (ifResetActCode)
        {
            actCode = ActCode.NONE;
        }
       

        if (Vector3.Distance(transform.position, initPos) > 0.2f)
        {
            if (attackCount!=0)
            {
                resting = true;
                debuffGo = Instantiate(hengSaoDebuffPrefab, transform);
                debuffGo.transform.localPosition = Vector3.zero;
            }
            SetMoveAction();
        }
        currentAttackCount = attackCount = 0;
    }
    /// <summary>
    /// 使用物品或者使用技能行为
    /// </summary>
    public void UseItemOrUseRemoteSkillBehaviour()
    {
        if (isPlayer)
        {
            AudioSourceManager.Instance.PlaySound("PlayerSkill");
        }
        else
        {
            AudioSourceManager.Instance.PlaySound("EnemySkill");
        }
        cac.PlaySkillAnimation();
    }
    /// <summary>
    /// 使用物品或者技能后的事件
    /// </summary>
    public void UseSkillOrItemAction()
    {     
        if (actCode==ActCode.USEITEM)//使用物品
        {
            switch (currentUseItemID)
            {
                case 0:
                    Instantiate(recoverHPEffectPrefab, cac.transform.position, Quaternion.identity);
                    break;
                case 1:
                    Instantiate(recoverMPEffectPrefab, cac.transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            Invoke("DelayUseItem",1.083f);
        }
        else if (actCode==ActCode.SKILL)//使用技能
        {
            AudioSourceManager.Instance.PlaySound(skillObj.soundPath);
            if (skillObj.skillType==SkillType.REMOTESINGLE)
            {
                Instantiate(GameController.Instance.skillPrefabGos[skillObj.ID], GameController.Instance.targetAI.cac.transform.position, Quaternion.identity);
            }
            else
            {
                GameController.Instance.GetAttackTargets(AOENum);
                ShowMPValueChange(-skillObj.decreateMPValue* GameController.Instance.playerTargetAIList.Count);
                for (int i = 0; i < GameController.Instance.playerTargetAIList.Count; i++)
                {
                    Instantiate(GameController.Instance.skillPrefabGos[skillObj.ID], GameController.Instance.playerTargetAIList[i].cac.transform.position, Quaternion.identity);
                }
            }
            Invoke("DelayHitEnemy", skillObj.delayHitTime);
        }
       
    }
    /// <summary>
    /// 延时使用物品
    /// </summary>
    public void DelayUseItem()
    {
        switch (currentUseItemID)
        {
            case 0:
                AudioSourceManager.Instance.PlaySound("RecoverHP");
                ShowHPValueChange(65);
                break;
            case 1:
                AudioSourceManager.Instance.PlaySound("RecoverMP");
                ShowMPValueChange(80);
                break;
            default:
                break;
        }
        Invoke("DelayEndCurrentTurn", 0.7f);
    }

    /// <summary>
    /// 延时对敌人造成伤害
    /// </summary>
    public void DelayHitEnemy()
    {
        if (skillObj.skillType==SkillType.REMOTESINGLE)
        {
            GameController.Instance.targetAI.HitBehaviour(false, false);
        }
        else
        {
            for (int i = 0; i < GameController.Instance.playerTargetAIList.Count; i++)
            {
                GameController.Instance.playerTargetAIList[i].HitBehaviour(false,false);
            }
        }
        Invoke("DelayEndCurrentTurn", 0.7f); 
    }

    /// <summary>
    /// 显示跟血量变化相关的内容
    /// </summary>
    public void ShowHPValueChange(int changeValue)
    {
        NumCanvas nc= Instantiate(numCanvas,cac.transform.position,Quaternion.identity);
        nc.ShowNum(changeValue);
        currentHP += changeValue;
        if (currentHP>=HP)
        {
            currentHP = HP;
        }
        characterCanvas.SetHPSliderValue((float)currentHP / HP);
        if (isPlayer)
        {
            PlayerManager.Instance.ChangeCurrentHP(changeValue);
            UIManager.Instance.SetHPSliderValue((float)currentHP / HP);
        }       
    }

    /// <summary>
    /// 显示跟蓝耗变化相关的内容
    /// </summary>
    public void ShowMPValueChange(int changeValue)
    {
        currentMP += changeValue;
        if (currentMP>=MP)
        {
            currentMP = MP;
        }
        if (isPlayer)
        {
            PlayerManager.Instance.ChangeCurrentMP(changeValue);
            UIManager.Instance.SetMPSliderValue((float)currentMP / MP);
        }
    }

    private void OnMouseDown()
    {
        if (IfCanClick())
        {
            GameController.Instance.playerTargetAI = this;
            GameMananger.Instance.SetCurrentCursorState(CursorIconState.NORMAL);
            if (UIManager.Instance.usingSkill)
            {
                //对这个目标使用技能
                UIManager.Instance.UseSKill();
            }
            else
            {
                //对这个目标进行攻击
                UIManager.Instance.ClickAttackBtn();
            }
        }
    }

    private void OnMouseOver()
    {
        spriteRenderer.material.SetColor("_Color",new Color(initColor.r*Mathf.Pow(2,1), initColor.g * Mathf.Pow(2, 1), initColor.b * Mathf.Pow(2, 1)));
        if (IfCanClick())
        {
            if (UIManager.Instance.usingSkill)
            {
                GameMananger.Instance.SetCurrentCursorState(CursorIconState.SKILL);
            }
            else
            {
                GameMananger.Instance.SetCurrentCursorState(CursorIconState.ATTACK);
            }
            
        }
        else
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GameMananger.Instance.SetCurrentCursorState(CursorIconState.FORBID);
            }           
        }
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.SetColor("_Color",initColor);
        GameMananger.Instance.SetCurrentCursorState(CursorIconState.NORMAL);
    }
    /// <summary>
    /// 当前敌人是否可以点击
    /// </summary>
    /// <returns></returns>
    public bool IfCanClick()
    {
        return gameObject.CompareTag("Enemy") && !GameController.Instance.isPerformingLogic && !EventSystem.current.IsPointerOverGameObject();
    }
    /// <summary>
    /// 延时结束当前回合
    /// </summary>
    private void DelayEndCurrentTurn()
    {
        EndCurrentTurn();
    }
    /// <summary>
    /// 销毁当前的debuff
    /// </summary>
    public void DestoryDebuff()
    {
        if (!resting)
        {
            if (debuffGo)
            {
                Destroy(debuffGo);
            }
        }
    }
}

public enum ActCode
{ 
    ATTACK,
    DEFEND,
    SKILL,
    USEITEM,
    NONE
}

public enum SkillType
{ 
    MELEE,
    REMOTESINGLE,
    REMOTEAOE
}
/// <summary>
/// 技能参数
/// </summary>
public struct SkillObj
{
    public int ID;
    public string name;
    public SkillType skillType;
    //public Vector3 attackPos;
    public float delayHitTime;
    public int decreateMPValue;
    public string soundPath;
}
