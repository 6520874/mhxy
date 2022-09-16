using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class GameMananger : MonoBehaviour
{
    public static GameMananger Instance { get; private set; }
    private float enterFightTimer;
    public bool canEnterFight;
    public GameObject normalModeGo;
    public GameObject fightModeGo;
    //public Transform playerNormalTrans;
    public CursorAnimation ca;
    private void Awake()
    {
        Instance = this;
        enterFightTimer = Time.time;
    }

    void Update()
    {
        if (canEnterFight)
        {
            JudgeEnterTheFight();
        }
    }

    private void JudgeEnterTheFight()
    {
        if (Time.time-enterFightTimer>=7)
        {
            if (Random.Range(0,5)>=1)
            {
                //进入战斗
                UIManager.Instance.SetMaterialValue(UIManager.Instance.CaptureCamera(Camera.main,new Rect(0,0,800,600)));
                SetEnterFightState(false);
                EnterOrExitFightMode(true);              
            }
            else
            {
                //重新计时
                SetEnterFightState(true);
            }
        }
    }
    /// <summary>
    /// 设置是否可以进入战斗的状态
    /// </summary>
    /// <param name="state"></param>
    public void SetEnterFightState(bool state)
    {
        canEnterFight = state;
        enterFightTimer = Time.time;
    }
    /// <summary>
    /// 设置正式进入战斗或非战斗状态
    /// </summary>
    public void EnterOrExitFightMode(bool enter)
    {
        normalModeGo.SetActive(!enter);
        fightModeGo.SetActive(enter);
        enterFightTimer = Time.time;
        canEnterFight = !enter;
        Vector3 pos = Camera.main.transform.position;
        pos.z = 0;
        fightModeGo.transform.position= pos;
        UIManager.Instance.ShowOrHidefightCommandPanelGo(enter);
        if (enter)
        {
            AudioSourceManager.Instance.PlayMusic("FightBG"+Random.Range(1,4));
        }
        else
        {
            AudioSourceManager.Instance.PlayMusic("Normal");
        }
    }
    /// <summary>
    /// 设置鼠标状态
    /// </summary>
    /// <param name="si"></param>
    public void SetCurrentCursorState(CursorIconState si)
    {
        ca.SetCurrentCursorState(si);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
