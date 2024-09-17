using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWindowUI : MonoBehaviour
{
    [SerializeField] private Transform template;
    [SerializeField] private Transform container;

    private Dictionary<Sprite, Transform> unitIconTransforms;  // Dictionary to store icon transforms
    private Dictionary<Sprite, int> goalUIParts;
    private void Awake()
    {
        EventAggregator.GetInstance().Subscribe<GoalsSetupEvent>(OnGoalsSetupEvent);
        EventAggregator.GetInstance().Subscribe<GoalsUpdateEvent>(OnGoalsUpdateEvent);
        unitIconTransforms = new Dictionary<Sprite, Transform>();
        template.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventAggregator.GetInstance().Unsubscribe<GoalsSetupEvent>(OnGoalsSetupEvent);
        EventAggregator.GetInstance().Unsubscribe<GoalsUpdateEvent>(OnGoalsUpdateEvent);
    }

    private void OnGoalsSetupEvent(GoalsSetupEvent e)
    {
        goalUIParts = e.GoalUIParts;
        foreach (KeyValuePair<Sprite, int> entry in e.GoalUIParts)
        {
            Sprite sprite = entry.Key;
            int count = entry.Value;

            Transform unitIconTransform = Instantiate(template, container);
            unitIconTransform.gameObject.SetActive(true);
            unitIconTransform.GetComponent<GoalUnitSingleUI>().SetVisual(sprite, count);

            unitIconTransforms[sprite] = unitIconTransform;
        }
    }

    private void OnGoalsUpdateEvent(GoalsUpdateEvent e)
    {
        foreach (KeyValuePair<Sprite, int> entry in e.parts)
        {
            Sprite sprite = entry.Key;
            int count = entry.Value;

            if (unitIconTransforms.TryGetValue(sprite, out Transform unitIconTransform))
            {
                int updatedValue = goalUIParts[sprite] - count;
                goalUIParts[sprite] = updatedValue;
                if (updatedValue <= 0)
                {
                    unitIconTransform.GetComponent<GoalUnitSingleUI>().GoalReachUpdateVisual();
                }
                else
                {
                    unitIconTransform.GetComponent<GoalUnitSingleUI>().SetVisual(sprite, updatedValue);
                }
            }
            else
            {
                Debug.LogWarning("Sprite not found in dictionary: " + sprite.name);
            }
        }
    }
}
