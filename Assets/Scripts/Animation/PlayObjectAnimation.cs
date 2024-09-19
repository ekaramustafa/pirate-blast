using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// Base class for Play/Non-UI objects Animations.
/// </summary>
public abstract class PlayObjectAnimation : MonoBehaviour
{
    [SerializeField] protected float defaultDuration = 0.25f;

    public abstract Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimation(float duration, Vector3 to, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes);

    public float GetDuration() => defaultDuration;


}

/// <summary>
/// Animation Type enums for GameObject Animations and UI Animations
/// </summary>
public enum AnimationType
{
    HORIZANTALSLIDE,
    VERTICALSLIDE,
    SCALE,
    SHAKEPOSITION,
    SHAKEROTATION,
    SHAKESCALE,
    HIGHLIGHT,
}