using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// Horizontal and Vertical sliding Animation for Play/Non-UI objects
/// </summary>
public class SlidingAnimation : PlayObjectAnimation
{

    /// <summary>
    /// Triggers a sliding animation starting from a specified position using the provided animation types.
    /// Uses the default duration for the animation.
    /// The target position is the current position.
    /// </summary>
    /// <param name="from">The starting position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(defaultDuration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a sliding animation with a specified duration, starting from a given position using the provided animation types.
    /// The target position is the current position.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="from">The starting position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimation(float duration, Vector3 from, params AnimationType[] animationTypes)
    {
        Vector3 currPos = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);

        foreach (AnimationType type in animationTypes)
        {
            switch (type)
            {
                case AnimationType.HORIZANTALSLIDE:
                    transform.localPosition = new Vector3(from.x, transform.localPosition.y, 0f);
                    break;

                case AnimationType.VERTICALSLIDE:
                    transform.localPosition = new Vector3(transform.localPosition.x, from.y, 0f);
                    break;
            }
        }

        return transform.DOLocalMove(currPos, duration, false).SetEase(Ease.OutBack, 0.5f);
    }

    /// <summary>
    /// Triggers a sliding animation towards a specified position using the provided animation types.
    /// Uses the default duration for the animation.
    /// The target position is the provided "to" Vector3 parameter.
    /// </summary>
    /// <param name="to">The target position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes)
    {
        return TriggerAnimationTo(defaultDuration, to, animationTypes);
    }

    /// <summary>
    /// Triggers a sliding animation towards a specified position with a given duration using the provided animation types.
    /// The target position is the provided "to" Vector3 parameter.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="to">The target position for the slide animation.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., horizontal or vertical slide).</param>
    /// <returns>A Tween object representing the sliding animation.</returns>
    public override Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes)
    {
        Vector3 destination = transform.position;
        foreach (AnimationType type in animationTypes)
        {
            switch (type)
            {
                case AnimationType.HORIZANTALSLIDE:
                    destination = new Vector3(to.x, destination.y, destination.z);
                    break;

                case AnimationType.VERTICALSLIDE:
                    destination = new Vector3(destination.x, to.y, destination.z);
                    break;
            }
        }
        return transform.DOLocalMove(destination, duration, false).SetEase(Ease.OutQuad, 0.5f);
    }
}



