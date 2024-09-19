using DG.Tweening;
using UnityEngine;

public class SlideAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        animatedTransform.localPosition = from;
        Vector3 newPos = new Vector3(to.x, to.y, from.z);
        return animatedTransform.DOLocalMove(newPos, duration).SetEase(Ease.OutBack, 0.5f);
    }
}