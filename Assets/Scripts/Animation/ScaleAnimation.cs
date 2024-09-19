using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Scales up and down animation. Bouncing effect
/// </summary>
public class ScaleAnimation : PlayObjectAnimation
{
    /// <summary>
    /// Triggers a scale animation using the specified Vector3 value and animation types. 
    /// Uses the serialized default duration for the animation.
    /// The target scale is the current scale.
    /// </summary>
    /// <param name="from">The Vector3 value used for scaling.</param>
    /// <param name="animationTypes">Array of animation types to be applied.</param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(defaultDuration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a scale animation with a specified duration, using the provided Vector3 value and animation types.
    /// The target scale is the current scale.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="from">The Vector3 value used for scaling.</param>
    /// <param name="animationTypes">Array of animation types to be applied.</param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimation(float duration, Vector3 from, params AnimationType[] animationTypes)
    {
        Vector3 targetScale = animationTransform.localScale * from.x;
        Vector3 currentScale = animationTransform.localScale;
        Sequence sequence = DOTween.Sequence();

        foreach (AnimationType type in animationTypes)
        {
            switch (type)
            {
                case AnimationType.SCALEUP:
                    animationTransform.localPosition = new Vector3(from.x, animationTransform.localPosition.y, 0f);
                    break;

                case AnimationType.SCALEDOWN:
                    animationTransform.localPosition = new Vector3(from.x, animationTransform.localPosition.y, 0f);
                    break;
            }
        }

        sequence.Append(animationTransform.DOScale(targetScale, duration / 2).SetEase(Ease.InExpo));
        sequence.Append(animationTransform.DOScale(currentScale, duration / 2).SetEase(Ease.InExpo));

        return sequence;
    }

    /// <summary>
    /// Triggers a scale animation to the specified Vector3 value using the given animation types.
    /// The target scale is the current scale.
    /// </summary>
    /// <param name="to">The Vector3 value used for scaling.</param>
    /// <param name="animationTypes">Array of animation types to be applied.</param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(defaultDuration, to, animationTypes);
    }

    /// <summary>
    /// Triggers a scale animation to the specified Vector3 value using the given animation types.
    /// The target scale is the current scale.
    /// Oscillation animations do not differentiate source and destination vectors
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="to"></param>
    /// <param name="animationTypes"></param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes)
    {
        animationTransform = GetComponent<Transform>();
        Vector3 targetScale = animationTransform.localScale * to.x;
        Vector3 currentScale = animationTransform.localScale;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(animationTransform.DOScale(targetScale, duration / 2).SetEase(Ease.InExpo));
        sequence.Append(animationTransform.DOScale(currentScale, duration / 2).SetEase(Ease.InExpo));

        return sequence;
    }
}
