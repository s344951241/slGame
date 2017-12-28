using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class DoTweenUtils{

    public delegate void CALL_BACK();
    public static CALL_BACK CallBack;

    public static Tween _tween;
    private static Ease _easeType = Ease.Linear;

    public static void SetEase(int easeType)
    {
        switch (easeType)
        {
            case 0:
                _easeType = Ease.Unset;
                break;

            default:
                _easeType = Ease.Unset;
                break;
        }
    }

    public static void DoScale(Transform trans, Vector3 endValue, float duration, TweenCallback fun)
    {
        _tween = trans.DOScale(endValue, duration);
        _tween.OnComplete(fun);
    }

    public static void DoValue(Slider slider, float endValue, float duration, TweenCallback fun)
    {
        _tween = DOTween.To(() => slider.value, x => slider.value = x, endValue, duration);
        _tween.OnComplete(fun);
    }

}
