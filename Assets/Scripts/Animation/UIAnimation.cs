using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Base class for UI Animations
/// </summary>
public abstract class UIAnimation : MonoBehaviour
{
    [SerializeField] protected float default_duration = 0.25f;
    protected RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public abstract Tween TriggerAnimation(Vector3 from, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimation(float duration, Vector3 val, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimationTo(Vector3 to, params AnimationType[] animationTypes);

    public abstract Tween TriggerAnimationTo(float duration, Vector3 to, params AnimationType[] animationTypes);

    public float GetDuration() => default_duration;


}

