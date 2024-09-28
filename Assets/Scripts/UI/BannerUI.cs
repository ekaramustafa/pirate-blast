using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerUI : MonoBehaviour
{
    private void Start()
    {
        Vector3 source = new Vector3(transform.localPosition.x, GameConstants.HEIGHT, 0);
        IAnimationService animationService = AnimationServiceLocator.GetUIAnimationService();
        Vector3 destination = transform.localPosition;
        transform.localPosition = source;
        animationService.TriggerAnimation(transform, source, destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2f, AnimationType.SLIDE);
    }
}
