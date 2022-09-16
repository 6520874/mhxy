using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    public static PlayerManager Instance { get;private set; }

    public int maxHP;
    public int currentHP;
    public int currentMaxHP;//伤势，血继上限
    public int maxMP;
    public int currentMP;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 修改当前血继上限
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentMaxHP(int value)
    {
        currentMaxHP += value;
    }
    /// <summary>
    /// 修改当前血量值
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentHP(int value)
    {
        currentHP += value;
    }
    /// <summary>
    /// 修改当前蓝量值
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentMP(int value)
    {
        currentMP += value;
    }
}
