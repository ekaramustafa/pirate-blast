using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class LevelSuccessUI : MonoBehaviour
{
    [SerializeField] private float sceneTransitionDelay = 2f;
    [SerializeField] private Transform levelCompletedText;
    [SerializeField] private Transform successImage;

    [Space(5)]
    [Header("Parameters")]
    [SerializeField] private string victoryText = "Victory";

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        IAnimationService UIanimationService = AnimationServiceLocator.GetUIAnimationService();
        Tween successTween = UIanimationService.TriggerAnimation(successImage.transform, successImage.transform.position, new Vector3(2f, 0f, 0f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        successTween.OnComplete(() =>
        {
            gameObject.GetComponent<ParticleSystem>().Play();
        });

        levelCompletedText.GetComponent<TextMeshProUGUI>().SetText(victoryText);
        Vector3 startPos = new Vector3(-GameConstants.WIDTH, 0f, 0f);
        levelCompletedText.GetComponent<UISlideAnimation>().TriggerAnimation(startPos, AnimationType.HORIZANTALSLIDE).OnComplete(() =>
        {
            StartCoroutine(DelayAndLoadMenu());
        });

    }

    private IEnumerator DelayAndLoadMenu()
    {


        yield return new WaitForSeconds(sceneTransitionDelay);
        if (successImage.gameObject.activeSelf)
        {
            successImage.GetComponent<UISlideAnimation>().TriggerAnimationTo(new Vector3(0f, -GameConstants.HEIGHT, 0f), AnimationType.VERTICALSLIDE);
        }
        levelCompletedText.GetComponent<UISlideAnimation>().TriggerAnimationTo(new Vector3(GameConstants.WIDTH, 0f, 0f), AnimationType.HORIZANTALSLIDE)
                .OnComplete(() => {
                    Loader.LoadMenu();
                });
    }

}
