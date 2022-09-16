using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NumCanvas : MonoBehaviour
{
    public Image[] numImages;
    public Sprite[] numSprites;
    public Vector3[] initPos;

    // Start is called before the first frame update
    void Start()
    {
        initPos = new Vector3[numImages.Length];
        for (int i = 0; i < initPos.Length; i++)
        {
            initPos[i] = numImages[i].transform.position;
        }
    }
    /// <summary>
    /// ÏÔÊ¾Êý×Ö
    /// </summary>
    /// <param name="num"></param>
    public void ShowNum(int num)
    {
        bool z = num >= 0;
        num = Mathf.Abs(num);

        int b = num / 100;
        numImages[0].sprite = numSprites[b+10*Convert.ToInt32(z)];
        numImages[0].gameObject.SetActive(b!=0);
        numImages[0].SetNativeSize();
        int s = (num % 100) / 10;
        numImages[1].sprite = numSprites[s + 10 * Convert.ToInt32(z)];
        if (b!=0)
        {
            numImages[1].gameObject.SetActive(true);
        }
        else
        {
            numImages[1].gameObject.SetActive(s!= 0);
        }
        numImages[1].SetNativeSize();
        int g = num % 10;
        numImages[2].gameObject.SetActive(true);
        numImages[2].sprite = numSprites[g + 10 * Convert.ToInt32(z)];
        numImages[2].SetNativeSize();

        int numIndex = 0;
        MoveNum(numIndex);
    }

    public void MoveNum(int index)
    {
        numImages[index].transform.DoMove(numImages[index].transform.position + new Vector3(0, 0.2f, 0), 0.1f)
            .SetOnComplete
            (
                () => 
                {
                    numImages[index].transform.DoMove(initPos[index], 0.1f).
                    SetOnComplete(
                            ()=>
                            {
                                index++;
                                if (index>2)
                                {
                                    for (int i = 0; i < numImages.Length; i++)
                                    {
                                        numImages[i].gameObject.SetActive(false);
                                    }
                                    return;
                                }
                                MoveNum(index);
                            }
                        );
                }
            );
    }
}
