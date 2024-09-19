using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MenuPopupUI : MonoBehaviour
{
    [SerializeField] private Transform animatedUITransform;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button playButton;
    [SerializeField] private Transform animatedPlayButton;

    [SerializeField] private Transform goalUI;



    private void Awake()
    {
        Hide();
        UpdateLevelText();
        playButton.enabled = true;
    }

    public void UpdateLevelText()
    {
        levelText.text = $"Level {GameConstants.CurrentLevel.ToString()}";
    }

    private void Start()
    {
       
    }

    public void LoadLevel()
    {
        if (playButton.enabled == false) return;
        playButton.enabled = false;
        animatedPlayButton.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(0.9f, 1f, 1f), AnimationType.SCALE).OnComplete(() =>
        {
            playButton.enabled = true;
            Hide();
            Loader.LoadLevel();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        goalUI.gameObject.SetActive(false);
    }

    public void Show()
    {
        UIScaleAnimation scaleAnimation = animatedUITransform.GetComponent<UIScaleAnimation>();
        scaleAnimation.TriggerAnimation(new Vector3(1.2f, 1f, 1f), AnimationType.SCALE).OnComplete(() => {
            goalUI.gameObject.SetActive(true);
            goalUI.GetComponent<MenuPopupGoalUI>().SpawnGoalObjects();
        });
    }

}
