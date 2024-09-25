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

        Tween tween = transform.DOMove(lastPosition, totalTime)
            .OnUpdate(() =>
            {
                if (index >= targetPositions.Count) return;

                Vector3 currentPosition = transform.position;
                Vector3 nextTargetPosition = targetPositions[index];
                float distanceToNextTarget = Vector3.Distance(currentPosition, nextTargetPosition);
                if (distanceToNextTarget <= Vector3.Distance(currentPosition, nextTargetPosition) * 0.5f)
                {
                    stepCallback?.Invoke(gameObject.GetComponent<Unit>(), currentPosition, nextTargetPosition);
                    index++;
                }
            }
            )
            .OnComplete(() =>
            {
                stepCallback?.Invoke(gameObject.GetComponent<Unit>(), transform.position, lastPosition);
                lastCallback?.Invoke(lastPosition);
            })
            .SetEase(Ease.OutBack, overshootAmount);

        return tween;
    }


}
