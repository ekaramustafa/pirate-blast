using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalUnitSingleUI : MonoBehaviour
{

    [SerializeField] private Transform iconContainter;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Transform goalCheckImage;


    private void Awake()
    {
        goalCheckImage.gameObject.SetActive(false);
    }

    public void SetVisual(Sprite sprite, int count)
    {

        iconTemplate.gameObject.SetActive(true);
        iconTemplate.GetComponent<Image>().sprite = sprite;
        countText.text = count.ToString();
       
    }


    public void GoalReachUpdateVisual()
    {
        goalCheckImage.gameObject.SetActive(true);
        countText.gameObject.SetActive(false);
    }

}
