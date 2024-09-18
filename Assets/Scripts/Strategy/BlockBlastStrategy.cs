using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class BlockBlastStrategy : IBlastStrategy
{
    public async UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition)
    {

        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        if (blastablePositions.Count >= GameConstants.TNT_FORMATION_BLOCKS_THRESHOLD)
        {
            await HandleTNTFormationBlast(gridSystem, startPosition, blastablePositions);
            return true;

        }
        else if (blastablePositions.Count >= GameConstants.BLAST_THRESHOLD)
        {
            await HandleBlockBlast(gridSystem, blastablePositions);
            return true;
        }
        return false;
    }

    private async Task HandleBlockBlast(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        await UniTask.Yield(); // there are no animations requiring blocking, for now...
        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        blastablePositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);
    }

    private async Task HandleTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> blastablePositions)
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
        await AnimateFormation(gridSystem, startPosition, blastablePositions);
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

    public async UniTask AnimateFormation(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> formedPositions)
    {
        Vector3 destination = gridSystem.GetWorldPosition(startPosition);
        Sequence parentSequence = DOTween.Sequence();

        foreach (GridPosition currentPosition in formedPositions)
        {
            if (currentPosition == startPosition) continue;

            GridObject gridObject = gridSystem.GetGridObject(currentPosition);
            Unit unit = gridObject.GetUnit();
            SlidingAnimation slidingAnimation = unit.GetComponent<SlidingAnimation>();
            Vector3 offset = GetOffsetVector(gridSystem, unit.transform.position, destination);
            Tween initialTween = slidingAnimation.TriggerAnimationTo(GameConstants.UNIT_FORM_ELASTICITIY_ANIMATION_DURATION, unit.transform.position + offset, AnimationType.HORIZANTALSLIDE, AnimationType.VERTICALSLIDE);
            Tween tween = slidingAnimation.TriggerAnimationTo(destination, AnimationType.HORIZANTALSLIDE, AnimationType.VERTICALSLIDE);
            Sequence subSequence = DOTween.Sequence();
            subSequence.Append(initialTween);
            subSequence.Append(tween);
            parentSequence.Join(subSequence);
        }

        // Await the completion of the animation sequence
        await parentSequence.AsyncWaitForCompletion();

    }

    private Vector3 GetOffsetVector(GridSystem gridSystem, Vector3 position, Vector3 destination)
    {
        GridPosition currentGridPosition = gridSystem.GetGridPosition(position);
        GridPosition destinationGridPosition = gridSystem.GetGridPosition(destination);
        float offsetCoefficient = GameConstants.UNIT_FORM_ELASTICITY_OFFSET;
        float yMultiplier = currentGridPosition.y - destinationGridPosition.y;
        float xMultiplier = currentGridPosition.x - destinationGridPosition.x;
        return new Vector3(offsetCoefficient * xMultiplier, offsetCoefficient * yMultiplier);

    }
}
