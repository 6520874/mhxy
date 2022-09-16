using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class TweenManager : MonoBehaviour
{
    private static TweenManager _instance;
    public static TweenManager Instance
    {
        get 
        {
            if (!_instance)
            {
                GameObject obj = new GameObject("TweenManager");
                _instance = obj.AddComponent<TweenManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}
