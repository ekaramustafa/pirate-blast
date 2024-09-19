using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelExitButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<UISlideAnimation>().TriggerAnimation(new Vector3(0f, -GameConstants.HEIGHT,0f), AnimationType.VERTICALSLIDE);
    }

    public void ReturnToMainMenu()
    {
        GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f, 0f, 0f), AnimationType.SCALEDOWN).OnComplete(() =>
        {
             Loader.LoadMenu();
        }); 

    }

}
