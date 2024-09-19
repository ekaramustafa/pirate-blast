using DG.Tweening;
using UnityEngine;

public interface IAnimationService
{
    Tween TriggerAnimation(Transform transform, Vector3 from, Vector3 to, float duration, AnimationType animationType);

}
