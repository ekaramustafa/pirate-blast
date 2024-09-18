using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
public class TNTBlastStrategy : IBlastStrategy
{

    public async UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> comboPositions = GridSearchUtils.GetAdjacentSameUnitType(gridSystem, startPosition);
        if (comboPositions.Count != 0)
        {
            await HandleComboTNTFormationBlast(gridSystem, startPosition, comboPositions);
            return true;
        }
        else
        {
            await HandleTNTBlast(gridSystem, startPosition);
            return true;
        }

    }

    private async UniTask HandleComboTNTFormationBlast(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> comboPositions)
    {
        await AnimateFormation(gridSystem, startPosition, comboPositions);
        foreach (GridPosition gridPosition in comboPositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, gridPosition, BlastType.TNTBlastForm);
        }
        gridSystem.GetUnitManager().CreateComboTNTUnit(startPosition);
        await AnimateComboTNTCreation(gridSystem, startPosition);
        BlastUtils.BlastBlockAtPosition(gridSystem, startPosition, BlastType.TNTBlast);
    }

    private async UniTask AnimateComboTNTCreation(GridSystem gridSystem, GridPosition startPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(startPosition);
        Unit unit = gridObject.GetUnit();
        unit.SetSortingOrder(999);
        ScaleAnimation scaleAnimation = unit.GetComponent<ScaleAnimation>();
        Tween tween = scaleAnimation.TriggerAnimationTo(0.5f,new Vector3(2f, 2f, 1f), AnimationType.SCALEUP);
        await tween.AsyncWaitForCompletion();
    }

    private async UniTask HandleTNTBlast(GridSystem gridSystem, GridPosition startPosition)
    {
        await UniTask.Yield();
        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
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

    private async UniTask AnimateFormation(GridSystem gridSystem, GridPosition startPosition, List<GridPosition> comboPositions)
    {
        Vector3 destination = gridSystem.GetWorldPosition(startPosition);
        Sequence parentSequence = DOTween.Sequence();

        foreach (GridPosition currentPosition in comboPositions)
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
