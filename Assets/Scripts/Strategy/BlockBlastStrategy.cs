using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Linq;

public class BlockBlastStrategy : IBlastStrategy
{
    public bool Blast(GridSystem gridSystem, GridPosition startPosition)
    {

        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        if (blastablePositions.Count >= GameConstants.TNT_FORMATION_BLOCKS_THRESHOLD)
        {
            HandleTNTFormationBlast(gridSystem, startPosition, blastablePositions);
            return true;

        }
        else if (blastablePositions.Count >= GameConstants.BLAST_THRESHOLD)
        {
            HandleBlockBlast(gridSystem, blastablePositions);
            return true;
        }
        return false;
    }

    private void HandleBlockBlast(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        blastablePositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);
        //Deactive the upper part, and drop them 
        //get the lowest row and col
        //Calculate left and right
        int startRow = blastablePositions.Min(pos => pos.y);
        int endRow = gridSystem.GetHeight();
        int startCol = blastablePositions.Min(pos => pos.x);
        int endCol = blastablePositions.Max(pos => pos.x) + 1;
        gridSystem.GetUnitManager().DropUnits(startRow, endRow, startCol, endCol);

    }

    private void HandleTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> blastablePositions)
    {
        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        List<GridPosition> mergedPositions = new List<GridPosition>(blastablePositions);
        mergedPositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, mergedPositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, mergedPositions);

        foreach (GridPosition position in neighbors)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }

        int startRow = mergedPositions.Min(pos => pos.y);
        int endRow = gridSystem.GetHeight();
        int startCol = mergedPositions.Min(pos => pos.x);
        int endCol = mergedPositions.Max(pos => pos.x) + 1;

        //gridSystem.GetUnitManager().DeActivateUnits(startRow, endRow, startCol, endCol); // For now, let's not be able to form a new unit when it is dropping.
        mergedPositions.ForEach(pos => gridSystem.GetGridObject(pos).SetIsInteractable(false));
        Tween sequence = AnimateFormation(gridSystem, startPosition, blastablePositions);
        UniTask task = sequence.AsyncWaitForCompletion().AsUniTask();

        List<Unit> unitsToBeDestoryed = blastablePositions.Select(pos => gridSystem.GetGridObject(pos).GetUnit()).ToList();

        TaskScheduler.EnqueueTaskBatch(
            new List<UniTask>(){ task }, 
            
            onStartCallback: () =>
            {
                Debug.Log("Form Animation Started");
            },
            
            onCompleteCallback : () =>
            {
                Debug.Log("Form Animation Finished");
                unitsToBeDestoryed.ForEach(unit => UnityEngine.GameObject.Destroy(unit.gameObject));
                gridSystem.GetUnitManager().DropUnits(startRow, endRow, startCol, endCol);
                mergedPositions.ForEach(pos => gridSystem.GetGridObject(pos).SetIsInteractable(true));
            }
        );

        blastablePositions.ForEach(pos => gridSystem.GetGridObject(pos).SetUnit(null));
        gridSystem.GetUnitManager().CreateTNTUnit(startPosition);
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);

    }

    public List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        return GridSearchUtils.GetAdjacentSameColorBlocks(gridSystem, startPosition);
    }

    /// <summary>
    /// This returns the list of grid positions that can be blasted by adjacent blast. 
    /// </summary>
    /// <param name="gridSystem"></param>
    /// <param name="blastablePositions"></param>
    public List<GridPosition> GetNeighborAffectableUnits(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        HashSet<GridPosition> visitedPositions = new HashSet<GridPosition>(blastablePositions);

        List<GridPosition> AllNeighborPositions = new List<GridPosition>();

        foreach (GridPosition gridPosition in blastablePositions)
        {
            List<GridPosition> neighborPositions = GridSearchUtils.GetNeighborBlastablePositions(gridSystem, gridPosition);

            foreach (GridPosition neighbor in neighborPositions)
            {
                if (!visitedPositions.Contains(neighbor))
                {
                    AllNeighborPositions.Add(neighbor);
                    visitedPositions.Add(neighbor);
                }
            }
        }

        return AllNeighborPositions;
    }

    public Tween AnimateFormation(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> formedPositions)
    {
        Vector3 destination = gridSystem.GetWorldPosition(startPosition);
        Sequence parentSequence = DOTween.Sequence();
        
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();

        foreach (GridPosition currentPosition in formedPositions)
        {
            if (currentPosition == startPosition) continue;

            GridObject gridObject = gridSystem.GetGridObject(currentPosition);
            Unit unit = gridObject.GetUnit();

            Vector3 offset = GetOffsetVector(gridSystem, unit.transform.position, destination);
            
            Tween initialTween = animationService.TriggerAnimation(unit.transform, unit.transform.position, unit.transform.position + offset, AnimationConstants.UNIT_FORM_ELASTICITIY_ANIMATION_DURATION, AnimationType.SLIDE);
            Tween tween = animationService.TriggerAnimation(unit.transform, unit.transform.position, destination, AnimationConstants.UNIT_FORM_FORWARD_ANIMATION_DURATION, AnimationType.SLIDE);
            
            Sequence subSequence = DOTween.Sequence();
            subSequence.Append(initialTween);
            subSequence.Append(tween);
            parentSequence.Join(subSequence);
        }
        return parentSequence;

    }

    private Vector3 GetOffsetVector(GridSystem gridSystem, Vector3 position, Vector3 destination)
    {
        GridPosition currentGridPosition = gridSystem.GetGridPosition(position);
        GridPosition destinationGridPosition = gridSystem.GetGridPosition(destination);
        float offsetCoefficient = AnimationConstants.UNIT_FORM_ELASTICITY_OFFSET;
        float yMultiplier = currentGridPosition.y - destinationGridPosition.y;
        float xMultiplier = currentGridPosition.x - destinationGridPosition.x;
        return new Vector3(offsetCoefficient * xMultiplier, offsetCoefficient * yMultiplier);

    }
}
