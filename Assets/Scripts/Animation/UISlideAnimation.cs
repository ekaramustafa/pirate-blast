using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// Horizontal and Vertical sliding Animation for UI Components. 
/// </summary>
public class UISlideAnimation : UIAnimation
{

    /// <summary>
    /// Triggers a sliding animation using the specified Vector3 value and animation types.
    /// Uses the serialized default duration for the animation.
    /// The target position is the current position.
    /// </summary>
    /// <param name="from">The Vector3 value used for positioning.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(default_duration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a sliding animation with a specified duration, using the provided Vector3 value and animation types.
    /// The target position is the current position.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="from">The Vector3 value used for positioning.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimation(float duration, Vector3 from, params AnimationType[] animationTypes)
    {
        Vector3 currPos = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);

        foreach (AnimationType type in animationTypes)
        {
            switch (type)
            {
                case AnimationType.HORIZANTALSLIDE:
                    rectTransform.localPosition = new Vector3(from.x, rectTransform.localPosition.y, 0f);
                    break;

                case AnimationType.VERTICALSLIDE:
                    rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, from.y, 0f);
                    break;
            }
        }

        return rectTransform.DOAnchorPos(currPos, duration, false).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Triggers a sliding animation to the specified Vector3 position using the given animation types.
    /// The target position is the provided "to" Vector3 parameter.
    /// </summary>
    /// <param name="to">The target Vector3 position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes)
    {
        return TriggerAnimationTo(default_duration, to, animationTypes);
    }

    /// <summary>
    /// Triggers a sliding animation to the specified Vector3 position using the given animation types.
    /// The target position is the provided "to" Vector3 parameter.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="to">The target Vector3 position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes)
    {
        foreach (AnimationType type in animationTypes)
        {
            switch (type)
            {
                case AnimationType.HORIZANTALSLIDE:
                    return rectTransform.DOAnchorPos(new Vector3(to.x, rectTransform.localPosition.y, 0f), duration, false).SetEase(Ease.OutQuad);

                case AnimationType.VERTICALSLIDE:
                    return rectTransform.DOAnchorPos(new Vector3(rectTransform.localPosition.x, to.y, 0f), duration, false).SetEase(Ease.OutQuad);
            }
        }
        return null;
    }
}



