using DG.Tweening;
using UnityEngine;

public class UIScaleBounceAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        RectTransform rectTransform = (RectTransform)animatedTransform;
        Vector3 targetScale = rectTransform.localScale * to.x;
        Vector3 currentScale = rectTransform.localScale;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(targetScale, duration).SetEase(Ease.OutBack));
        sequence.Append(rectTransform.DOScale(currentScale, duration).SetEase(Ease.OutBack));

        return sequence;
    }
}
