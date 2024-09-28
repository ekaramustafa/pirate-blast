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

    private IAnimationService UIanimationService;

    private void Awake()
    {
        gameObject.SetActive(false);
        UIanimationService = AnimationServiceLocator.GetUIAnimationService();
    }

    private void OnEnable()
    {
        Tween successTween = UIanimationService.TriggerAnimation(successImage.transform, successImage.transform.position, new Vector3(2f, 0f, 0f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        successTween.OnComplete(() =>
        {
            gameObject.GetComponent<ParticleSystem>().Play();
        });

        levelCompletedText.GetComponent<TextMeshProUGUI>().SetText(victoryText);
        Vector3 sourcePos = new Vector3(-GameConstants.WIDTH, levelCompletedText.transform.localPosition.y, 0f);
        Vector3 destinationPos = levelCompletedText.localPosition;
        levelCompletedText.localPosition = sourcePos;
        UIanimationService.TriggerAnimation(levelCompletedText.transform, sourcePos, destinationPos, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2f, AnimationType.SLIDE)
            .OnComplete(() =>
            {
                StartCoroutine(DelayAndLoadMenu());
            });

    }

    private IEnumerator DelayAndLoadMenu()
    {


        yield return new WaitForSeconds(sceneTransitionDelay);

        
        if (successImage.gameObject.activeSelf)
        {
            UIanimationService.TriggerAnimation(successImage, successImage.localPosition, new Vector3(successImage.localPosition.x, -GameConstants.HEIGHT, 0f), AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2f, AnimationType.SLIDE);
        }
        UIanimationService.TriggerAnimation(levelCompletedText, levelCompletedText.localPosition, new Vector3(GameConstants.WIDTH, levelCompletedText.localPosition.y, 0f), AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2f, AnimationType.SLIDE)
            .OnComplete(() =>
            {
                Loader.LoadMenu();
            });
    }

}
