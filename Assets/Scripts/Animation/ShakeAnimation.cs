using DG.Tweening;
using UnityEngine;
/// <summary>
/// Shake animation for Play/Non-UI objects.
/// </summary>
public class ShakeAnimation : PlayObjectAnimation
{

    /// <summary>
    /// Triggers a shake animation starting from a specified position using the provided animation types.
    /// Uses the default duration for the animation.
    /// </summary>
    /// <param name="from">The strength of the shake animation, represented as a Vector3.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., shake position, shake rotation, shake scale).</param>
    /// <returns>A Tween object representing the shake animation sequence.</returns>
    public override Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(defaultDuration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a shake animation with a specified duration, using the provided strength and animation types.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="from">The strength of the shake animation, represented as a Vector3.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., shake position, shake rotation, shake scale).</param>
    /// <returns>A Tween object representing the shake animation sequence.</returns>
    public override Tween TriggerAnimation(float duration, Vector3 from, params AnimationType[] animationTypes)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var animationType in animationTypes)
        {
            switch (animationType)
            {
                case AnimationType.SHAKEPOSITION:
                    sequence.Append(transform.DOShakePosition(defaultDuration, strength: from));
                    break;
                case AnimationType.SHAKEROTATION:
                    sequence.Append(transform.DOShakeRotation(defaultDuration, strength: from));
                    break;
                case AnimationType.SHAKESCALE:
                    sequence.Append(transform.DOShakeScale(defaultDuration, strength: from));
                    break;
            }
        }

        return sequence;
    }

    /// <summary>
    /// Triggers a shake animation towards a specified position using the provided animation types.
    /// Uses the default duration for the animation.
    /// Oscillation animations do not differentiate source and destination vectors.
    /// </summary>
    /// <param name="from">The strength of the shake animation, represented as a Vector3.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., shake position, shake rotation, shake scale).</param>
    /// <returns>A Tween object representing the shake animation sequence.</returns>
    public override Tween TriggerAnimationTo(Vector3 from, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(defaultDuration, from, animationTypes);
    }

    /// <summary>
    /// Triggers a shake animation towards a specified position with a given duration using the provided animation types.
    /// Oscillation animations do not differentiate source and destination vectors.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="to">The strength of the shake animation, represented as a Vector3.</param>
    /// <param name="animationTypes">Array of animation types to be applied (e.g., shake position, shake rotation, shake scale).</param>
    /// <returns>A Tween object representing the shake animation sequence.</returns>
    public override Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes)
    {
        return TriggerAnimation(duration, to, animationTypes);
    }
}
