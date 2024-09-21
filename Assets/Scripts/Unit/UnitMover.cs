using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UnitMover : MonoBehaviour
{
    [SerializeField] private float overshootAmount = 1f;
    


    public UniTask MoveWithDOTween(Vector3 targetPosition, float totalTime)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float maxSpeed = 40f;
        totalTime = Mathf.Max(totalTime, distance/maxSpeed);

        Vector3 overshootPosition = targetPosition + (targetPosition - startPosition).normalized * overshootAmount;
        Vector3 undershootPosition = targetPosition - (targetPosition - startPosition).normalized * (overshootAmount * 0.2f);

        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Append(transform.DOMove(overshootPosition, totalTime * 0.3f).SetEase(Ease.OutQuad));

        moveSequence.Append(transform.DOMove(undershootPosition, totalTime * 0.3f).SetEase(Ease.OutQuad));

        moveSequence.Append(transform.DOMove(targetPosition, totalTime * 0.4f).SetEase(Ease.OutQuad));

        return moveSequence.AsyncWaitForCompletion().AsUniTask();
    }


    public IEnumerator MoveCoroutine(Vector3 targetPosition, float speed)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float totalTime = distance / speed;  // Calculate time based on speed and distance
        float elapsedTime = 0f;

        Vector3 overshootPosition = targetPosition + (targetPosition - startPosition).normalized * overshootAmount;
        Vector3 undershootPosition = targetPosition - (targetPosition - startPosition).normalized * (overshootAmount * 0.5f); // Undershoot for added bounce

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            float easedT = EaseInQuad(t);

            if (easedT < 0.3f)
            {
                // Move quickly towards the overshoot position
                transform.position = Vector3.Lerp(startPosition, overshootPosition, easedT / 0.3f);
            }
            else if (easedT < 0.6f)
            {
                // Move slowly back to the undershoot position
                float slowT = (easedT - 0.3f) / 0.3f;
                transform.position = Vector3.Lerp(overshootPosition, undershootPosition, EaseOutQuad(slowT));
            }
            else
            {
                // Move very slowly to the final target position
                float finalT = (easedT - 0.6f) / 0.4f;
                transform.position = Vector3.Lerp(undershootPosition, targetPosition, EaseOutQuad(finalT));
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    /// <summary>
    /// Eases out with a quadratic function, creating a decelerating effect.
    /// This function takes an input value `t` (ranging from 0 to 1) and returns `t * (2 - t)`,
    /// which starts fast and then slows down as it approaches the end.
    /// The result is a smooth finish, with the movement decelerating towards the target.
    /// </summary>
    /// <param name="t">The normalized time (0 to 1) representing the current progress of the easing.</param>
    /// <returns>A float representing the eased value at the given time `t`.</returns>
    private float EaseOutQuad(float t)
    {
        return t * (2 - t); // Easier, slower ending
    }

    // <summary>
    /// Eases in with a quadratic function, creating an accelerating effect.
    /// This function takes an input value `t` (ranging from 0 to 1) and returns `t` squared.
    /// The result is a smooth start, with the movement starting slowly and accelerating as it progresses.
    /// </summary>
    /// <param name="t">The normalized time (0 to 1) representing the current progress of the easing.</param>
    /// <returns>A float representing the eased value at the given time `t`.</returns>
    private float EaseInQuad(float t)
    {
        return t * t;
    }

}
