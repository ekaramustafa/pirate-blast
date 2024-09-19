using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Scales up and down animation. Bouncing effect
/// </summary>
public class UIScaleAnimation :  UIAnimation
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
        return TriggerAnimation(default_duration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a scale animation with a specified duration, using the provided Vector3 value and animation types.
    /// The target scale is the current scale.
    /// 
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="from">The Vector3 value used for scaling.</param>
    /// <param name="animationTypes">Array of animation types to be applied.</param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimation(float duration, Vector3 from, params AnimationType[] animationTypes)
    {
        Vector3 targetScale = rectTransform.localScale * from.x;
        Vector3 currentScale = rectTransform.localScale;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(targetScale, duration / 2).SetEase(Ease.InExpo));
        sequence.Append(rectTransform.DOScale(currentScale, duration / 2).SetEase(Ease.InExpo));

        return sequence;
    }

    /// <summary>
    /// Triggers a scale animation to the specified Vector3 value using the given animation types.
    /// The target scale is the current scale.
    /// Oscillation animations do not differentiate source and destination vectors.
    /// </summary>
    /// <param name="to">The Vector3 value used for scaling.</param>
    /// <param name="animationTypes">Array of animation types to be applied.</param>
    /// <returns>A Tween object representing the animation sequence.</returns>
    public override Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(to, animationTypes); //Gotta implement this
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
        return TriggerAnimation(duration, to, animationTypes);
    }
}
