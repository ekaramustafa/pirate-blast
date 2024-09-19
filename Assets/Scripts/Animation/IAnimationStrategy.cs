using DG.Tweening;
using UnityEngine;

public interface IAnimationStrategy
{
    Tween Animate(Transform animatedTransform, Vector3 from, Vector3 to, float duration);
}