using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：鼠标动画
//***************************************** 
public class CursorAnimation : MonoBehaviour
{
    public Texture2D[] texture2Ds;
    private int cursorIndex;
    private float setCursorTimer;
    public CursorIconState iconState;
    public Texture2D attackIcon;
    public Texture2D forbidIcon;
    public Texture2D skillIcon;
    void Start()
    {
        Cursor.SetCursor(texture2Ds[cursorIndex],Vector2.zero,CursorMode.Auto);
    }

    void Update()
    {
        if (iconState==CursorIconState.NORMAL)
        {
            if (Time.time - setCursorTimer >= 0.1f)
            {
                cursorIndex++;
                if (cursorIndex >= texture2Ds.Length)
                {
                    cursorIndex = 0;
                }
                Cursor.SetCursor(texture2Ds[cursorIndex], Vector2.zero, CursorMode.Auto);
                setCursorTimer = Time.time;
            }
        }
       
    }

    public void SetCurrentCursorState(CursorIconState si)
    {
        iconState = si;
        if (iconState==CursorIconState.ATTACK)
        {
            Cursor.SetCursor(attackIcon, Vector2.zero, CursorMode.Auto);
        }
        else if (iconState == CursorIconState.FORBID)
        {
            Cursor.SetCursor(forbidIcon, Vector2.zero, CursorMode.Auto);
        }
        else if (iconState==CursorIconState.SKILL)
        {
            Cursor.SetCursor(skillIcon, Vector2.zero, CursorMode.Auto);
        }
    }
}

public enum CursorIconState
{ 
    NORMAL,
    ATTACK,
    FORBID,
    SKILL
}
