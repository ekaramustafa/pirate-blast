using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Linq;

public class BlockBlastStrategy : IBlastStrategy
{
    public static int semaphore = 0;

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
        RequestManager requestManager = gridSystem.GetRequestManager();
        UnitManager unitManager = gridSystem.GetUnitManager();

        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        blastablePositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);

        UserRequest userRequest = new UserRequest(blastablePositions.ToArray(), async (request) =>
        {
            await unitManager.DropUnits(request);
            requestManager.FinishCallback(request);

        });
        requestManager.PostRequest(userRequest);
        requestManager.FinishRequest(userRequest);
    }

    private void HandleTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> blastablePositions)
    {
        RequestManager requestManager = gridSystem.GetRequestManager();
        UnitManager unitManager = gridSystem.GetUnitManager();

        List<GridPosition> neighbors = GetNeighborAffectableUnits(gridSystem, blastablePositions);
        List<GridPosition> mergedPositions = new List<GridPosition>(blastablePositions);
        mergedPositions.AddRange(neighbors);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, mergedPositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, mergedPositions);

        foreach (GridPosition position in neighbors)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
        }

        List<Unit> unitsToBeDestoryed = blastablePositions.Select(pos => gridSystem.GetGridObject(pos).GetUnit()).ToList();
        UserRequest userRequest = new UserRequest(mergedPositions.ToArray(), async (request) =>
        {
            unitsToBeDestoryed.ForEach(unit => {
                if (unit != null)
                {
                    DOTween.Kill(unit.transform.gameObject);
                    UnityEngine.GameObject.Destroy(unit.gameObject);
                }

            });
            await unitManager.DropUnits(request);
            requestManager.FinishCallback(request);
        });

        requestManager.PostRequest(userRequest);

        Tween sequence = AnimateFormation(gridSystem, startPosition, blastablePositions);
        sequence.OnComplete(() =>
        {
            unitManager.CreateTNTUnit(startPosition);
            requestManager.FinishRequest(userRequest);
        });

        blastablePositions.ForEach(pos =>
        {
            if(pos != startPosition)
            {
                gridSystem.GetGridObject(pos).SetUnit(null);
            }
        });

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
