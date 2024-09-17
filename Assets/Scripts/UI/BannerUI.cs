using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerUI : MonoBehaviour
{
    private void Start()
    {
        Vector3 pos = new Vector3(0, GameConstants.HEIGHT, 0);
        GetComponent<UISlideAnimation>().TriggerAnimation(pos, AnimationType.VERTICALSLIDE);
    }
}
