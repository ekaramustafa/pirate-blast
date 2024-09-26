using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlastUtils
{
    public static void BlastBlockAtPosition(GridSystem gridSystem, GridPosition position, BlastType blastType)
    {
        if (!gridSystem.CanPerformOnPosition(position)) return;

        GridObject gridObject = gridSystem.GetGridObject(position);
        gridObject.HitUnit(blastType);
    }

    public static void PublishBlastedParts(GridSystem gridSystem, Dictionary<GridPosition, Sprite> positionSpriteMap, Dictionary<Sprite, int> spriteCountMap)
    {
        foreach (KeyValuePair<GridPosition, Sprite> kvp in positionSpriteMap)
        {
            GridPosition position = kvp.Key;
            Sprite sprite = kvp.Value;
            if (gridSystem.CanPerformOnPosition(position))
            {
                spriteCountMap[sprite]--;
            }
            
        }

        EventAggregator.GetInstance().Publish(new GoalsUpdateEvent(spriteCountMap));
    }

    public static Dictionary<GridPosition, Sprite> GetBlastedPositionsSpriteMap(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        Dictionary<GridPosition, Sprite> map = new Dictionary<GridPosition, Sprite>();
        foreach (GridPosition position in blastablePositions)
        {
            if (!gridSystem.CanPerformOnPosition(position)) continue;

            GridObject gridObject = gridSystem.GetGridObject(position);
            Unit unit = gridObject.GetUnit();
            UnitType unitType = unit.GetUnitType();
            if (unitType == UnitType.Block || unitType == UnitType.TNT)
                continue;
            Sprite sprite = unit.GetDefaultSprite();
            map[position] = sprite;
        }
        return map;
    }

    public static Dictionary<Sprite, int> GetBlastedSpritesCountMap(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        Dictionary<Sprite, int> map = new Dictionary<Sprite, int>();
        HashSet<GridPosition> visitedGridPositions = new HashSet<GridPosition>();
        foreach (GridPosition position in blastablePositions)
        {
            if (visitedGridPositions.Contains(position))
                continue;
            visitedGridPositions.Add(position);
            if (!gridSystem.CanPerformOnPosition(position)) continue;
            GridObject gridObject = gridSystem.GetGridObject(position);
            Unit unit = gridObject.GetUnit();
            UnitType unitType = unit.GetUnitType();

            if (unitType == UnitType.Block || unitType == UnitType.TNT)
                continue;

            Sprite sprite = unit.GetDefaultSprite();
            if (map.ContainsKey(sprite))
            {
                map[sprite] += 1;
            }
            else
            {
                map[sprite] = 1;
            }
        }
        return map;
    }
}
