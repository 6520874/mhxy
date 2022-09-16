using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class NotWalkableArea : MonoBehaviour
{
    public CharacterAI characterAI;

    private void OnMouseDown()
    {
        characterAI.willStopping = true;
    }
}
