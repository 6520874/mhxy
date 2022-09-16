using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecoverHPMP : MonoBehaviour,IPointerClickHandler
{
    public bool recoverHP;
    public GameObject recoverHPEffectPrefab;
    public GameObject recoverMPEffectPrefab;
    public Transform normalPlayerTrans;

    public void RecoverHP()
    {
        PlayerManager.Instance.currentHP = PlayerManager.Instance.maxHP;
        UIManager.Instance.SetHPSliderValue(1);
        AudioSourceManager.Instance.PlaySound("RecoverHP");
        GameObject go= Instantiate(recoverHPEffectPrefab,normalPlayerTrans);
        go.transform.localPosition = Vector3.zero;
    }

    public void RecoverMP()
    {
        PlayerManager.Instance.currentMP = PlayerManager.Instance.maxMP;
        UIManager.Instance.SetMPSliderValue(1);
        AudioSourceManager.Instance.PlaySound("RecoverMP");
        GameObject go = Instantiate(recoverMPEffectPrefab, normalPlayerTrans);
        go.transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button==PointerEventData.InputButton.Right&&!GameMananger.Instance.fightModeGo.activeSelf)
        {
            if (recoverHP)
            {
                RecoverHP();
            }
            else
            {
                RecoverMP();
            }
        };
    }
}
