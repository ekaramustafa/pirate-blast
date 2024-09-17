using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuPopupGoalUI : MonoBehaviour
{
    [SerializeField] private Transform template;
    [SerializeField] private Transform container;
    [SerializeField] private UnitAssetsData unitAssetsSO;
    [SerializeField] private Transform goalsText;

    private LevelData levelData;

    private void Awake()
    {
        goalsText.gameObject.SetActive(false);
        template.gameObject.SetActive(false);
        levelData = LevelDataLoaderWriter.LoadLevelData();
    }

    public void SpawnGoalObjects()
    {
        foreach (Transform child in container.transform)
        {
           if(child != template || child != goalsText)
            {
                Destroy(child.gameObject);
            }
        }

        goalsText.gameObject.SetActive(true);
        goalsText.GetComponent<UIScaleAnimation>().TriggerAnimation(new Vector3(1.2f, 1f, 1f), AnimationType.SCALEUP);
        Dictionary<Sprite, int> map = new Dictionary<Sprite, int>();

        string[] data_array = levelData.grid;

        foreach (string data in data_array)
        {
            UnitType unitType = MappingUtils.stringToUnitTypeMapping[data]();
            if (unitType == UnitType.Block || unitType == UnitType.TNT) continue;

            UnitData unitSO = unitAssetsSO.GetUnitSOByUnitType(unitType);


            if (unitSO != null)
            {
                Sprite sprite = unitSO.defaultStateSprite;

                if (sprite != null)
                {
                    if (map.ContainsKey(sprite))
                    {
                        map[sprite] += 1;
                    }
                    else
                    {
                        map[sprite] = 1;

                    }
                }
            }
        }

        foreach (KeyValuePair<Sprite, int> entry in map)
        {
            Sprite sprite = entry.Key;
            int count = entry.Value;

            Transform unitIconTransform = Instantiate(template, container);
            unitIconTransform.gameObject.SetActive(true);
            unitIconTransform.GetComponent<GoalUnitSingleUI>().SetVisual(sprite, count);
            UIScaleAnimation UIScaleAnimation = unitIconTransform.GetComponent<UIScaleAnimation>();
            UIScaleAnimation.TriggerAnimation(new Vector3(1.2f, 1f, 1f), AnimationType.SCALEUP).WaitForCompletion();
        }
    }

    private void Hide()
    {

    }

}
