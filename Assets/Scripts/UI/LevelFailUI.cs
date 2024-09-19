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

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UIScaleAnimation scaleAnimation = GetComponent<UIScaleAnimation>();
        scaleAnimation.TriggerAnimation(new Vector3(1.2f, 1f, 1f), AnimationType.SCALE);
    }

    #region Button Functions
    public void TryAgain()
    {
        animatedTryAgainButton.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f, 1f, 1f), AnimationType.SCALE).OnComplete(() =>
        {
            Loader.LoadLevel();
        });
    }

    public void ReturnToMainMenu()
    {
        animatedExitButton.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f,1f,1f), AnimationType.SCALE).OnComplete( () =>
        {
            Loader.LoadMenu();
        });
    }
    #endregion
}
