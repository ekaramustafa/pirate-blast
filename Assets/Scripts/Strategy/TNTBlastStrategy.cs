using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class TNTBlastStrategy : IBlastStrategy
{

    public bool Blast(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> comboPositions = GridSearchUtils.GetAdjacentSameUnitType(gridSystem, startPosition);
        if (comboPositions.Any(pos => !gridSystem.GetGridObject(pos).IsInteractable())) return false;

        if (comboPositions.Count >= GameConstants.TNT_COMBO_FORMATION_THRESHOLD)
        {
            HandleComboTNTFormationBlast(gridSystem, startPosition, comboPositions);
        }
        else
        {
            HandleTNTBlast(gridSystem, startPosition);
        }
        return true;


    }

    private void HandleComboTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> comboPositions)
    {
        RequestManager requestManager = gridSystem.GetRequestManager();
        UnitManager unitManager = gridSystem.GetUnitManager();

        List<GridPosition> affectedNeighbors = new List<GridPosition>();
        foreach (GridPosition pos in comboPositions)
        {
            affectedNeighbors.AddRange(GridSearchUtils.GetNeighborBlastablePositions(gridSystem, pos));
        }
        foreach (GridPosition pos in affectedNeighbors)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, pos, BlastType.BlockBlast);
        }

        List<Unit> unitsToBeDestoryed = comboPositions.Select(pos => gridSystem.GetGridObject(pos).GetUnit()).ToList();

        int startCol = 0;
        int endCol = gridSystem.GetWidth();
        int startRow = 0;
        int endRow = gridSystem.GetHeight();
        unitManager.DeActivateUnits(startRow, endRow, startCol, endCol); //Deactivate all interactions

        Tween sequence = AnimateFormation(gridSystem, startPosition, comboPositions);
        sequence.OnComplete(() =>
        {
            unitsToBeDestoryed.ForEach(unit => {
                if(unit != null)
                {
                    UnityEngine.GameObject.Destroy(unit.gameObject);
                }
            });
            unitManager.CreateComboTNTUnit(startPosition);
            AnimateComboTNTCreation(gridSystem, startPosition).OnComplete(() =>
            {
                HandleTNTBlast(gridSystem, startPosition);
                unitManager.ActivateUnits(startRow, endRow, startCol, endCol);
            });
        });

        comboPositions.ForEach(pos =>
        {
            gridSystem.GetGridObject(pos).SetIsInteractable(false);
            if (pos != startPosition)
            {
                gridSystem.GetGridObject(pos).SetUnit(null);
            }
        });

        
    }

    private Tween AnimateComboTNTCreation(GridSystem gridSystem, GridPosition startPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(startPosition);
        Unit unit = gridObject.GetUnit();
        unit.SetSortingOrder(999);
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        Vector3 targetScale = new Vector3(5f, 5f, 1f);
        Tween scalingUpTween = animationService.TriggerAnimation(unit.transform, unit.transform.localScale, targetScale, 0.25f, AnimationType.SCALE);
        return scalingUpTween;
    }

    private void HandleTNTBlast(GridSystem gridSystem, GridPosition startPosition)
    {
        RequestManager requestManager = gridSystem.GetRequestManager();
        UnitManager unitManager = gridSystem.GetUnitManager();

        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.TNTBlast);
        }

        UserRequest userRequest = new UserRequest(blastablePositions.ToArray(), async (request) =>
        {
            await unitManager.DropUnits(request);
            requestManager.FinishCallback(request);
        });

        requestManager.PostRequest(userRequest);
        requestManager.FinishRequest(userRequest);
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);

    }

    private Tween AnimateFormation(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> comboPositions)
    {
        Vector3 destination = gridSystem.GetWorldPosition(startPosition);
        Sequence parentSequence = DOTween.Sequence();
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();

        foreach (GridPosition currentPosition in comboPositions)
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

    public List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        //For TNT Blast
        List<GridPosition> blastablePositions = new List<GridPosition>();
        Queue<GridPosition> queue = new Queue<GridPosition>();
        HashSet<GridPosition> visited = new HashSet<GridPosition>();
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            GridPosition currentPosition = queue.Dequeue();

            if (visited.Contains(currentPosition))
            {
                continue;
            }
            visited.Add(currentPosition);
            GridObject currentGridObject = gridSystem.GetGridObject(currentPosition);
            TNTData tntSO = currentGridObject.GetUnit().GetUnitData() as TNTData;
            int range = tntSO.range;
            for (int xOffset = -range; xOffset <= range; xOffset++)
            {
                for (int yOffset = -range; yOffset <= range; yOffset++)
                {
                    GridPosition blastPosition = new GridPosition(currentPosition.x + xOffset, currentPosition.y + yOffset);
                    if (gridSystem.CanPerformOnPosition(blastPosition))
                    {
                        blastablePositions.Add(blastPosition);
                        GridObject gridObject = gridSystem.GetGridObject(blastPosition);
                        UnitType unitType = gridObject.GetUnit().GetUnitType();
                        if (unitType == UnitType.TNT && !visited.Contains(blastPosition))
                        {
                            queue.Enqueue(blastPosition);
                        }
                    }
                }
            }
        }

        return blastablePositions;
    }

    


}
