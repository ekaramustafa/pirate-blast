using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class LevelCompletedUI : MonoBehaviour
{
    [SerializeField] private Transform successUI;
    [SerializeField] private Transform failUI;

    private void Awake()
    {
        EventAggregator.GetInstance().Subscribe<LevelFinishedEvent>(OnLevelFinished);
    }

    private void OnDestroy()
    {
        EventAggregator.GetInstance().Unsubscribe<LevelFinishedEvent>(OnLevelFinished);
    }

    private void OnLevelFinished(LevelFinishedEvent e)
    {
        if (e.success)
        {
            GameConstants.CurrentLevel += 1;
            successUI.gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(WaitAndLoad());
            
        }
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(1f);
        failUI.gameObject.SetActive(true);

    }

}
