using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class TNTBlastStrategy : IBlastStrategy
{

    public bool Blast(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> comboPositions = GridSearchUtils.GetAdjacentSameUnitType(gridSystem, startPosition);
        if (comboPositions.Any(pos => !gridSystem.GetGridObject(pos).IsInteractable())) return false;

        if (comboPositions.Count != 0)
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
        int startRow = 0;
        int endRow = gridSystem.GetHeight();
        int startCol = comboPositions.Min(pos => pos.x);
        int endCol = comboPositions.Max(pos => pos.x) + 1;
        gridSystem.GetUnitManager().DeActivateUnits(startRow, endRow, startCol, endCol);


        AnimateFormation(gridSystem, startPosition, comboPositions).OnComplete(() =>
        {
            foreach (GridPosition gridPosition in comboPositions)
            {
                BlastUtils.BlastBlockAtPosition(gridSystem, gridPosition, BlastType.TNTBlastForm);
            }
            gridSystem.GetUnitManager().CreateComboTNTUnit(startPosition);
            AnimateComboTNTCreation(gridSystem, startPosition).OnComplete(() =>
            {
                HandleTNTBlast(gridSystem, startPosition);
            });
        });
        
    }

    private Tween AnimateComboTNTCreation(GridSystem gridSystem, GridPosition startPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(startPosition);
        Unit unit = gridObject.GetUnit();
        unit.SetSortingOrder(999);
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        Sequence seq = DOTween.Sequence();
        Vector3 targetScale = new Vector3(5f, 5f, 1f);
        Vector3 sourceScale = unit.transform.localScale;
        Tween scalingUpTween = animationService.TriggerAnimation(unit.transform, unit.transform.localScale, targetScale, 0.25f, AnimationType.SCALE);
        //Tween scalingDownTween = animationService.TriggerAnimation(unit.transform, unit.transform.localScale, sourceScale, 0.25f, AnimationType.SCALE);

        seq.Append(scalingUpTween);
        //seq.Append(scalingDownTween);
        return seq;
    }

    private void HandleTNTBlast(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        
        int startRow = 0;
        int endRow = gridSystem.GetHeight();
        int startCol = blastablePositions.Min(pos => pos.x);
        int endCol = blastablePositions.Max(pos => pos.x) + 1;
        gridSystem.GetUnitManager().DeActivateUnits(startRow, endRow, startCol, endCol);

        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.TNTBlast);
        }
        blastablePositions.Clear();
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);
    }

    public List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        //For TNT
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


}
