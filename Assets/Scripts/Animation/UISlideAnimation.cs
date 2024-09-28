using DG.Tweening;
using UnityEngine;

public class UISlideAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        RectTransform rectTransform = (RectTransform)animatedTransform;
        return rectTransform.DOAnchorPos(to, duration).SetEase(Ease.OutQuad);
    }
}
