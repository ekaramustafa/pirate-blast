using DG.Tweening;
using UnityEngine;

public class NewScaleAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        animatedTransform.localScale = from;
        Vector3 newPos = new Vector3(to.x, to.y, from.z);
        return animatedTransform.DOScale(newPos, duration).SetEase(Ease.InExpo);
    }
}
