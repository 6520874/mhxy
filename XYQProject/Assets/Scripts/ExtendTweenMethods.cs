using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：拓展动画方法的工具类
//***************************************** 
public static class ExtendTweenMethods 
{
    /// <summary>
    /// 动画
    /// </summary>
    public class Tween
    {
        public Coroutine tweenCoroutine;//动画执行逻辑的协程
        public int loopCount;//动画循环次数
        public int currentLoopCount;//当前循环到的次数
        public Vector3 originalValue;//起始值
        public Vector3 targetValue;//目标值
        public TweenMethodType tweenMethodType;//根据调用方法不同进行不同的处理（比如初始化）
        public Transform transform;//作用的transform对象
        public float time;
        public bool isPause;
        public bool autoKill;
        public delegate void Callback();
        public Callback onComplete;
        public Callback onKill;
        public Callback onPause;

        #region 设置动画属性
        public Tween(TweenMethodType type,Transform trans,Vector3 targetVal,float tweenTime,int loopCout=1)
        {
            tweenMethodType = type;
            transform = trans;
            targetValue = targetVal;
            time = tweenTime;
            loopCount = loopCout;
            currentLoopCount = 0;
            isPause = false;
            autoKill = true;
            tweenCoroutine = null;
            onComplete= onKill= onPause= null;
            SetOrignalValue();
        }
        /// <summary>
        /// 获取初始值
        /// </summary>
        public void SetOrignalValue()
        {
            switch (tweenMethodType)
            {
                case TweenMethodType.DOMOVE:
                    originalValue = transform.position;
                    break;
                case TweenMethodType.DOROTATE:
                    originalValue = transform.eulerAngles;
                    break;
                case TweenMethodType.DOSCALE:
                    originalValue = transform.localScale;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 设置初始值
        /// </summary>
        public void ResetState()
        {
            switch (tweenMethodType)
            {
                case TweenMethodType.DOMOVE:
                    transform.position=originalValue;
                    break;
                case TweenMethodType.DOROTATE:
                    transform.eulerAngles=originalValue;
                    break;
                case TweenMethodType.DOSCALE:
                    transform.localScale=originalValue;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 设置是否自动销毁动画
        /// </summary>
        /// <param name="auto"></param>
        public Tween SetAutoKill(bool auto)
        {
            autoKill = auto;
            return this;
        }
        /// <summary>
        /// 设置动画中的协程
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public Tween SetCoroutine(Coroutine coroutine)
        {
            tweenCoroutine = coroutine;
            return this;
        }
        #endregion

        #region 设置回调方法
        /// <summary>
        /// 设置动画完成后的回调方法
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Tween SetOnComplete(Callback c)
        {
            onComplete += c;
            return this;
        }
        /// <summary>
        /// 设置动画销毁后的回调方法
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Tween SetOnKill(Callback c)
        {
            onKill += c;
            return this;
        }
        /// <summary>
        /// 设置动画暂停后的回调方法
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Tween SetOnPause(Callback c)
        {
            onPause += c;
            return this;
        }

        #endregion

        #region 回调方法
        /// <summary>
        /// 完成动画后的回调
        /// </summary>
        public void OnComplete()
        {
            if (onComplete!=null)
            {
                onComplete();
            }
            if (autoKill)
            {
                Kill();
            }
        }
        /// <summary>
        /// 销毁动画后的回调
        /// </summary>
        public void OnKill()
        {
            if (onKill!=null)
            {
                onKill();
            }
        }
        /// <summary>
        /// 暂停动画后的回调
        /// </summary>
        public void OnPause()
        {
            if (onPause!=null)
            {
                OnPause();
            }
        }
        #endregion

        #region 改变动画状态
        /// <summary>
        /// 销毁动画
        /// </summary>
        public void Kill()
        {
            TweenManager.Instance.StopCoroutine(tweenCoroutine);
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        public void Play()
        {
            isPause = false;
        }
        /// <summary>
        /// 暂停动画
        /// </summary>
        public void Pause()
        {
            isPause = true;
        }
        /// <summary>
        /// 重播动画
        /// </summary>
        public void ReStart()
        {
            ResetState();
            Play();
        }

        #endregion
    }
    /// <summary>
    /// 动画执行逻辑协程
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="tween"></param>
    /// <returns></returns>
    public static IEnumerator TweenLogicCoroutine(this MonoBehaviour mono,Tween tween)
    {
        for (tween.currentLoopCount = 0; tween.currentLoopCount < tween.loopCount; tween.currentLoopCount++)
        {
            tween.ResetState();
            //动画总体插值
            Vector3 targetTotalValue = (tween.targetValue - tween.originalValue) / tween.time;
            for (float t=tween.time;t>=0;t-=Time.deltaTime)
            {
                //上边计算的动画插值每一帧都进行更新
                ChangeEveryFrame(tween, targetTotalValue*Time.deltaTime);
                yield return null;
                while (tween.isPause)
                {
                    yield return null;
                }
            }        
        }
        tween.OnComplete();
    }
    /// <summary>
    /// 更新每一帧的变化
    /// </summary>
    public static void ChangeEveryFrame(Tween tween,Vector3 changedValue)
    {
        switch (tween.tweenMethodType)
        {
            case TweenMethodType.DOMOVE:
                tween.transform.Translate(changedValue);
                break;
            case TweenMethodType.DOROTATE:
                tween.transform.Rotate(changedValue);
                break;
            case TweenMethodType.DOSCALE:
                tween.transform.localScale += changedValue;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 移动动画方法
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="time"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public static Tween DoMove(this Transform transform,Vector3 target,float time,int loop=1)
    {
        Tween tween = new Tween(TweenMethodType.DOMOVE,transform,target,time,loop);
        Coroutine coroutine = TweenManager.Instance.StartCoroutine(TweenManager.Instance.TweenLogicCoroutine(tween));
        tween.SetCoroutine(coroutine);
        return tween;
    }
}

public enum TweenMethodType
{ 
    DOMOVE,
    DOROTATE,
    DOSCALE
}
