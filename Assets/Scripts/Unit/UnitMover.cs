using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class UnitMover : MonoBehaviour
{

    public Tween MoveWithDOTween(List<Vector3> targetPositions, float totalTime, float overshootAmount, Action<Unit, Vector3, Vector3> stepCallback, Action<Vector3> lastCallback)
    {
        if (targetPositions == null || targetPositions.Count == 0)
            return null;

        Vector3 lastPosition = targetPositions[targetPositions.Count - 1];
        int index = 0;
        AnimationCurve smoothBounceCurve = new AnimationCurve(
            new Keyframe(0f, 0f),   // Starting point
            new Keyframe(0.6f, 1.1f),  // Reduced peak of bounce (overshoot)
            new Keyframe(1f, 1f)    // Ending point
        );
        Vector3 startPosition = transform.position;

        Tween tween = transform.DOMove(lastPosition, totalTime).SetId(gameObject)
            .OnUpdate(() =>
            {
                if (index >= targetPositions.Count) return;

                Vector3 currentPosition = transform.position;
                Vector3 nextTargetPosition = targetPositions[index];
                float totalDistance = Vector3.Distance(startPosition, nextTargetPosition);
                float distanceToNextTarget = Vector3.Distance(currentPosition, nextTargetPosition);
                if (distanceToNextTarget < totalDistance * 0.5f)
                {
                    stepCallback?.Invoke(gameObject.GetComponent<Unit>(), currentPosition, nextTargetPosition);
                    index++;
                    startPosition = currentPosition;
                }
            }
            )
            .OnComplete(() =>
            {
                stepCallback?.Invoke(gameObject.GetComponent<Unit>(), transform.position, lastPosition);
                lastCallback?.Invoke(lastPosition);
            })
            .OnKill(() =>
            {
                lastCallback?.Invoke(lastPosition);
            }).SetEase(smoothBounceCurve);
        return tween;
    }


}
