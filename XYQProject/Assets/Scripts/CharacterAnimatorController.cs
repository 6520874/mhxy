using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：控制人物动画切换
//***************************************** 
public class CharacterAnimatorController : MonoBehaviour
{
    public Animator animator;
    public bool isMoving;
    public CharacterFightAI fightAI;

    public void PlayLocomotionAnimation(Vector3 currentPos,Vector3 navPos,Vector3 targetPos)
    {
        Vector3 lookDir = navPos-currentPos;
        if (lookDir.magnitude<=0.0001f)
        {
            lookDir = targetPos - currentPos;
        }
        if (Vector3.Distance(currentPos,targetPos)>0.3f)
        {
            animator.SetFloat("LookX", lookDir.normalized.x);
            animator.SetFloat("LookY", lookDir.normalized.y);
            animator.SetBool("MoveState",true);
            isMoving = true;
        }
        else if(Vector3.Distance(currentPos, targetPos) < 0.15f)
        {
            animator.SetBool("MoveState", false);
            isMoving = false;
        }
    }

    public void PlayHitAnimation()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDieAnimation()
    {
        animator.SetBool("Die",true);
    }

    public void PlayMoveAnimation(float moveValue)
    {
        animator.SetFloat("MoveX",moveValue);
        animator.SetInteger("MoveValue",1);
    }

    public void PlayIdleAnimation()
    {
        animator.SetInteger("MoveValue", 0);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void PlaySkillAnimation()
    {
        animator.SetTrigger("Skill");
    }

    public void PlayDefendAnimation()
    {
        animator.SetTrigger("Defend");
    }


    #region 动画事件
    private void AttackAnimationEvent()
    {
        fightAI.AttackTarget();       
    }
    private void UseSkillOrItemAnimationEvent()
    {
        fightAI.UseSkillOrItemAction();
    }

    #endregion

}
