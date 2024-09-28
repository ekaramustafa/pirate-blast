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
        Vector3 source = new Vector3(transform.localPosition.x, -GameConstants.HEIGHT, 0f);
        Vector3 destination = transform.localPosition;
        transform.localPosition = source;
        animationService.TriggerAnimation(transform,source, destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2f, AnimationType.SLIDE);
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
