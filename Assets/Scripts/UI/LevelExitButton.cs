using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelExitButton : MonoBehaviour
{
    private IAnimationService animationService;
    private void Start()
    {
        animationService = AnimationServiceLocator.GetUIAnimationService();
        GetComponent<UISlideAnimation>().TriggerAnimation(new Vector3(0f, -GameConstants.HEIGHT,0f), AnimationType.VERTICALSLIDE);
    }

    public void ReturnToMainMenu()
    {
        Tween tween = animationService.TriggerAnimation(transform, transform.position, new Vector3(0.9f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        tween.OnComplete(() =>
        {
             Loader.LoadMenu();
        }); 

    }

}
