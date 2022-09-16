using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战斗中人物的相关UI显示
/// </summary>
public class CharacterCanvas : MonoBehaviour
{
    public Slider hpSlider;

    private void Start()
    {
        SetHPSliderValue((float)PlayerManager.Instance.currentHP/PlayerManager.Instance.currentMaxHP);
    }

    public void SetHPSliderValue(float value)
    {
        hpSlider.value = value;
    }
}
