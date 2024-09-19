using DG.Tweening;
using UnityEngine;

public class ShakeRotationAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        Tween tween = animatedTransform.DOShakeRotation(duration, strength: to);
        tween.OnComplete(() =>
        {
            animatedTransform.localScale = from;
        });
        return tween;
    }
}
