using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class MenuUI : MonoBehaviour
{
    [SerializeField] private Transform headline;
    [SerializeField] private Transform popupUI;
    [Space(5)]
    [Header("Buttons & Texts")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelButton;
    [SerializeField] private Transform animatedLevelButton;
    
    [SerializeField] private Transform animatedResetButton; 
    [SerializeField] private Button resetButton; //For resetting level counts


    IAnimationService animationService;
    private void Start()
    {
        levelButton.enabled = true;
        if (GameConstants.CurrentLevel > GameConstants.MAX_LEVEL)
        {
            GameConstants.CurrentLevel = GameConstants.MAX_LEVEL;
            levelText.text = "Finished";
            levelButton.enabled = false;
        }
        else
        {
            levelText.text = $"Level {GameConstants.CurrentLevel.ToString()}";
        }
        animationService = AnimationServiceLocator.GetUIAnimationService();
        Vector3 destination = headline.transform.localPosition;
        headline.transform.localPosition = new Vector3Int(0, GameConstants.HEIGHT, 0);
        animationService.TriggerAnimation(headline.transform, headline.transform.localPosition, destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION / 2, AnimationType.SLIDE);
    }

    public void LoadPopupUI()
    {
        if (levelButton.enabled == false) return;
        if (popupUI.gameObject.activeSelf) return;
        levelButton.enabled = false;
        Tween tween = animationService.TriggerAnimation(animatedLevelButton.transform, animatedLevelButton.transform.position, new Vector3(0.9f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        tween.OnComplete(() =>
        {
            popupUI.gameObject.SetActive(true);
            popupUI.GetComponent<MenuPopupUI>().Show();
            levelButton.enabled = true;
        });
    }

    public void ResetLevelCount()
    {
        if (resetButton.enabled == false) return;
        GameConstants.CurrentLevel = 1;
        levelButton.enabled = false;
        resetButton.enabled = false;
        Tween tween = animationService.TriggerAnimation(animatedResetButton.transform, animatedResetButton.transform.position, new Vector3(0.9f, 1f, 1f), AnimationConstants.SCALEBOUNCE_DEFAULT_DURATION, AnimationType.SCALEBOUNCE);
        tween.OnComplete(() =>
        {
            resetButton.enabled = true;
            levelButton.enabled = true;
        });

        levelText.text = $"Level {GameConstants.CurrentLevel.ToString()}";
        popupUI.GetComponent<MenuPopupUI>().UpdateLevelText();
    }
    

}
