using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： Trigger 
//功能说明：非战斗状态下的寻路
//***************************************** 
public class CharacterAI : MonoBehaviour
{
    public NavMeshAgent meshAgent;
    private Vector3 targetPos;
    public CharacterAnimatorController cac;
    public bool willStopping;
    public GameObject testTarget;
    public GameObject clickEffectGo;
    private float followMouseTimer;
    private int clickCount;
    public bool followMouse;
    private float createEffectTimer;

    private void OnEnable()
    {
        targetPos = transform.position;
    }

    void Start()
    {
        meshAgent.updateRotation = false;
        meshAgent.updateUpAxis = false;
        targetPos=transform.position;
        meshAgent.updatePosition = false;
    }

    void Update()
    {
        cac.PlayLocomotionAnimation(transform.position,meshAgent.nextPosition,targetPos);
        SetNavAgentState();
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            followMouse = false;
            ClickMouse();
        }
        GetNotWalkableAreaMovePoint();
        DoubleClickMouse();
        testTarget.transform.position = targetPos;
        transform.position = meshAgent.nextPosition;
    }

    private void SetNavAgentState()
    {
        meshAgent.isStopped = !cac.isMoving;
    }

    private void ClickMouse()
    {
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = transform.position.z;
        meshAgent.SetDestination(targetPos);
        if (Time.time-createEffectTimer>=0.05f)
        {
            createEffectTimer = Time.time;
            Instantiate(clickEffectGo, targetPos, Quaternion.identity);
        }       
    }

    private void GetNotWalkableAreaMovePoint()
    {
        if (willStopping)
        {
            Ray2D ray = new Ray2D(transform.position, targetPos - transform.position);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (raycastHit2D)
            {
                targetPos = raycastHit2D.point;
                targetPos -= 0.1f * (targetPos - transform.position);
            }
            willStopping = false;
            targetPos.z = transform.position.z;
            meshAgent.SetDestination(targetPos);
        }
    }

    private void DoubleClickMouse()
    {
        if (followMouse)//开启开关，人物跟随鼠标移动
        {
            ClickMouse();
        }
        else
        {
            if (Time.time-followMouseTimer>=0.4f)
            {
                //已超出规定时间，重新计时
                followMouseTimer = Time.time;
                clickCount = 0;
            }
            else
            {
                //在时间间隔内
                if (clickCount>1)
                {
                    //双击
                    followMouse = true;
                }
            }
        }
    }
}
