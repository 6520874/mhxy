using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviour
{
    public Transform[] bgTrans;
    public Vector3[] targetPos;
    public ExtendTweenMethods.Tween[] tweens;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = new Vector3[bgTrans.Length];
        for (int i = 0; i < bgTrans.Length; i++)
        {
            targetPos[i] = bgTrans[i].position;
        }
        tweens = new ExtendTweenMethods.Tween[2];
        tweens[0]= bgTrans[0].DoMove(targetPos[2],100,100);
        tweens[1] = bgTrans[1].DoMove(targetPos[2], 50, 1).SetOnComplete(() =>        
        {
            bgTrans[1].position = targetPos[0];
            bgTrans[1].DoMove(targetPos[2], 100, 100);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            tweens[i].Kill();
        }
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
