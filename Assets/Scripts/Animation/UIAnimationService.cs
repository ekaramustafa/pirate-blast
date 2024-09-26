using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationService : IAnimationService
{
    private readonly Dictionary<AnimationType, IAnimationStrategy> _UIanimationStrategies;

    public UIAnimationService()
    {
        _UIanimationStrategies = new Dictionary<AnimationType, IAnimationStrategy>
        {
            { AnimationType.SCALEBOUNCE, new UIScaleBounceAnimation()},
            {AnimationType.SLIDE, new NewUISlideAnimation() }
        };
    }

    public Tween TriggerAnimation(Transform transform, Vector3 from, Vector3 to, float duration, AnimationType animationType)
    {
        if (_UIanimationStrategies.TryGetValue(animationType, out var strategy))
        {
            return strategy.Animate(transform, from, to, duration);
        }

        throw new ArgumentException($"No animation strategy found for {animationType}");

    }
}

public class NewUISlideAnimation : IAnimationStrategy
{
    public Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration)
    {
        RectTransform rectTransform = (RectTransform)animatedTransform;
        return rectTransform.DOAnchorPos(to, duration, false).SetEase(Ease.OutQuad);
    }
}
