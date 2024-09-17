using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class MenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelButton;
    [SerializeField] private Transform animatedLevelButton;
    [SerializeField] private Transform popupUI;

    [SerializeField] private Transform animatedResetButton; //For resetting level counts
    [SerializeField] private Button resetButton;
    
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
    }

    public void LoadPopupUI()
    {
        if (levelButton.enabled == false) return;
        if (popupUI.gameObject.activeSelf) return;
        levelButton.enabled = false;
        animatedLevelButton.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f,1f,1f), AnimationType.SCALEDOWN).OnComplete(() =>
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
        animatedResetButton.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f, 1f, 1f), AnimationType.SCALEDOWN).OnComplete(() =>
        {
            resetButton.enabled = true;
            levelButton.enabled = true;
        });
        levelText.text = $"Level {GameConstants.CurrentLevel.ToString()}";
        popupUI.GetComponent<MenuPopupUI>().UpdateLevelText();
    }
    

}
