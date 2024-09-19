using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelFailUI : MonoBehaviour
{
    [SerializeField] private Transform banner;

    [Space(5)]
    [Header("Animated Component of Buttons")]
    [SerializeField] private Transform animatedTryAgainButton;
    [SerializeField] private Transform animatedExitButton;

    IAnimationService animationService;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        animationService = AnimationServiceLocator.GetUIAnimationService();
        animationService.TriggerAnimation(transform, transform.position, new Vector3(1.2f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
    }

    #region Button Functions
    public void TryAgain()
    {
        Tween tween = animationService.TriggerAnimation(transform, transform.position, new Vector3(0.9f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        tween.OnComplete(() =>
        {
            Loader.LoadLevel();
        });
    }

    public void ReturnToMainMenu()
    {
        Tween tween = animationService.TriggerAnimation(transform, transform.position, new Vector3(0.9f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        tween.OnComplete(() =>
        {
            Loader.LoadMenu();
        });
    }
    #endregion
}
