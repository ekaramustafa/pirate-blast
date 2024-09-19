using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationService : IAnimationService
{
    private readonly Dictionary<AnimationType, IAnimationStrategy> _animationStrategies;

    public AnimationService()
    {
        _animationStrategies = new Dictionary<AnimationType, IAnimationStrategy>
        {
            { AnimationType.SLIDE, new SlideAnimation() },
            { AnimationType.SCALE, new NewScaleAnimation() },
            /*
            
            { AnimationType.SHAKEPOSITION, new ShakePositionAnimation() },
            // Add more animation types here
            */
        };
    }

    public Tween TriggerAnimation(Transform transform, Vector3 from, Vector3 to, float duration, AnimationType animationType)
    {
        if (_animationStrategies.TryGetValue(animationType, out var strategy))
        {
            return strategy.Animate(transform, from,to,duration);
        }

        throw new ArgumentException($"No animation strategy found for {animationType}");

    }
}
