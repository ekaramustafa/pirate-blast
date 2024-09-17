using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockFormNewUnitStrategy : IFormNewUnitStrategy
{
    //For forming TNT from blocks
    private GridSystem gridSystem;
    public BlockFormNewUnitStrategy(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    public async UniTask<bool> Form(GridPosition startPosition, UnitData unitSO)
    {
        List<GridPosition> formablePositions = GetFormablePositions(startPosition);
        if (formablePositions.Count < GameConstants.TNT_FORMATION_BLOCKS_THRESHOLD)
            return false;

        await AnimateFormation(startPosition, formablePositions);
        ClearFormablePositions(formablePositions);
        gridSystem.GetUnitManager().CreateUnit(startPosition, unitSO);
        return true;
    }

    public void ClearFormablePositions(List<GridPosition> formablePositions)
    {
        foreach (GridPosition gridPosition in formablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, gridPosition, BlastType.None);
        }

    }

    public List<GridPosition> GetFormablePositions(GridPosition startPosition)
    {
        return GridSearchUtils.GetAdjacentSameColorBlocks(gridSystem, startPosition);
    }

    public async UniTask AnimateFormation(GridPosition startPosition, List<GridPosition> formedPositions)
    {
        Vector3 destination = gridSystem.GetWorldPosition(startPosition);
        Sequence mySequence = DOTween.Sequence();

        foreach (GridPosition currentPosition in formedPositions)
        {
            if (currentPosition == startPosition) continue;

            GridObject gridObject = gridSystem.GetGridObject(currentPosition);
            Unit unit = gridObject.GetUnit();
            SlidingAnimation slidingAnimation = unit.GetComponent<SlidingAnimation>();
            Tween tween = slidingAnimation.TriggerAnimationTo(destination, AnimationType.HORIZANTALSLIDE, AnimationType.VERTICALSLIDE);
            mySequence.Join(tween);
        }

        // Await the completion of the animation sequence
        await mySequence.AsyncWaitForCompletion();
    }
}
