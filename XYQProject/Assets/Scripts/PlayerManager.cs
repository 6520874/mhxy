using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    public static PlayerManager Instance { get;private set; }

    public int maxHP;
    public int currentHP;
    public int currentMaxHP;//���ƣ�Ѫ������
    public int maxMP;
    public int currentMP;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// �޸ĵ�ǰѪ������
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentMaxHP(int value)
    {
        currentMaxHP += value;
    }
    /// <summary>
    /// �޸ĵ�ǰѪ��ֵ
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentHP(int value)
    {
        currentHP += value;
    }
    /// <summary>
    /// �޸ĵ�ǰ����ֵ
    /// </summary>
    /// <param name="value"></param>
    public void ChangeCurrentMP(int value)
    {
        currentMP += value;
    }
}
