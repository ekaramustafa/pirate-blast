using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoveWindowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;

    private int count;
    private void Awake()
    {
        EventAggregator.GetInstance().Subscribe<MoveConsumedEvent>(OnMoveConsumed);
        EventAggregator.GetInstance().Subscribe<MoveSetupEvent>(OnMoveSetup);
    }

    private void OnDestroy()
    {
        EventAggregator.GetInstance().Unsubscribe<MoveConsumedEvent>(OnMoveConsumed);
        EventAggregator.GetInstance().Unsubscribe<MoveSetupEvent>(OnMoveSetup);
    }

    private void Start()
    {
        UpdateMovesUI();
    }

    private void OnMoveSetup(MoveSetupEvent e)
    {
        count = e.Moves;
        UpdateMovesUI();
    }


    private void OnMoveConsumed(MoveConsumedEvent e)
    {
        count -= e.MovesConsumed;

        UpdateMovesUI();
        
    }

    private void UpdateMovesUI()
    {
        countText.text = count.ToString();
    }
}
