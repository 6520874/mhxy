using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ս������������UI��ʾ
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
