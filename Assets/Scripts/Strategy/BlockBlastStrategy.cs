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
        int endRow = gridSystem.GetHeight() - 1;
        int startCol = blastablePositions.Min(pos => pos.x);
        int endCol = blastablePositions.Max(pos => pos.x);
        gridSystem.GetUnitManager().DropUnits(startRow, endRow, startCol, endCol).Forget();

    }

    private void HandleTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> blastablePositions)
    {
        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        List<GridPosition> blastedPositions = new List<GridPosition>(blastablePositions);
        blastedPositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastedPositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastedPositions);

        foreach (GridPosition position in neighbors)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }
        AnimateFormation(gridSystem, startPosition, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlastForm);
        }

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

    public void AnimateFormation(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> formedPositions)
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

        // Await the completion of the animation sequence
        //await parentSequence.AsyncWaitForCompletion();

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
