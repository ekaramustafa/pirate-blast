using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class TNTFormNewUnitStrategy : IFormNewUnitStrategy
{

    //For Forming TNT Combo
    private GridSystem gridSystem;
    public TNTFormNewUnitStrategy(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    public async UniTask<bool> Form(GridPosition startPosition, UnitData unitSO)
    {
        await UniTask.Yield(); // there are no animations requiring blocking, for now...

        List<GridPosition> formablePositions = GetFormablePositions(startPosition);
        if (formablePositions.Count < GameConstants.TNT_COMBO_FORMATION_THRESHOLD)
            return false;

      
        foreach (GridPosition gridPosition in formablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, gridPosition, BlastType.None);
        }
        gridSystem.GetUnitManager().CreateUnit(startPosition, unitSO);
        return true;
    }

    public List<GridPosition> GetFormablePositions(GridPosition startPosition)
    {
        List<GridPosition> formablePositions = GridSearchUtils.GetAdjacentSameUnitType(gridSystem, startPosition);
        return formablePositions;
    }

    public UniTask AnimateFormation(GridPosition gridPosition, List<GridPosition> formedPositions)
    {
        throw new NotImplementedException("No animation is implemented for TNT-TNT Combo Formation yet");
    }


}

