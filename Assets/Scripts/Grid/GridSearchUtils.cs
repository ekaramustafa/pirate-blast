using System;
using System.Collections.Generic;
using UnityEngine;

public static class GridSearchUtils
{

    public static List<GridPosition> GetNeighborBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> neighborPositions = new List<GridPosition>();

        GridPosition[] directions = {
            new GridPosition(-1, 0),  // left
            new GridPosition(1, 0),   // right
            new GridPosition(0, 1),   // top
            new GridPosition(0, -1)   // bottom
        };

        foreach (GridPosition direction in directions)
        {
            GridPosition neighborPosition = new GridPosition(
                startPosition.x + direction.x,
                startPosition.y + direction.y
            );
            
            if (gridSystem.CanPerformOnPosition(neighborPosition) && gridSystem.CanBeAffectedByNeighborBlast(neighborPosition))
            {
                neighborPositions.Add(neighborPosition);
            }
        }

        return neighborPositions;
    }

    public static List<GridPosition> GetAdjacentSameUnitType(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> sameUnitTypePositions = new List<GridPosition>();
        GridObject startGridObject = gridSystem.GetGridObject(startPosition);

        UnitType startUnitType = startGridObject.GetUnit().GetUnitType();
        Stack<GridPosition> stack = new Stack<GridPosition>();
        HashSet<GridPosition> visited = new HashSet<GridPosition>();

        stack.Push(startPosition);

        while (stack.Count > 0)
        {
            GridPosition currentPosition = stack.Pop();

            if (visited.Contains(currentPosition))
            {
                continue;
            }

            visited.Add(currentPosition);
            sameUnitTypePositions.Add(currentPosition);

            // Define directions to check: left, right, top, bottom
            GridPosition[] directions = {
                new GridPosition(-1, 0),  // left
                new GridPosition(1, 0),   // right
                new GridPosition(0, 1),   // top
                new GridPosition(0, -1)   // bottom
            };

            foreach (GridPosition direction in directions)
            {
                GridPosition adjacentPosition = new GridPosition(currentPosition.x + direction.x, currentPosition.y + direction.y);

                if (gridSystem.CanPerformOnPosition(adjacentPosition))
                {
                    GridObject adjacentGridObject = gridSystem.GetGridObject(adjacentPosition);
                    UnitType adjacentUnitType = adjacentGridObject.GetUnit().GetUnitType();
                    if (adjacentUnitType == startUnitType && !visited.Contains(adjacentPosition))
                    {
                        stack.Push(adjacentPosition);
                    }
                }
            }
        }

        return sameUnitTypePositions;
    }

    public static List<GridPosition> GetAdjacentSameColorBlocks(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> sameColorPositions = new List<GridPosition>();
        GridObject startGridObject = gridSystem.GetGridObject(startPosition);
        UnitType unitType = startGridObject.GetUnit().GetUnitType();

        if (unitType != UnitType.Block)
        {
            return sameColorPositions;
        }

        UnitData unitSO = startGridObject.GetUnit().GetUnitData();
        BlockData startBlockSO = (BlockData)unitSO;
        BlockColor startColor = startBlockSO.blockColor;
        Stack<GridPosition> stack = new Stack<GridPosition>();
        HashSet<GridPosition> visited = new HashSet<GridPosition>();

        stack.Push(startPosition);

        while (stack.Count > 0)
        {
            GridPosition currentPosition = stack.Pop();

            if (visited.Contains(currentPosition))
            {
                continue;
            }

            visited.Add(currentPosition);
            sameColorPositions.Add(currentPosition);

            // Define directions to check: left, right, top, bottom
            GridPosition[] directions = {
                new GridPosition(-1, 0),  // left
                new GridPosition(1, 0),   // right
                new GridPosition(0, 1),   // top
                new GridPosition(0, -1)   // bottom
            };

            foreach (GridPosition direction in directions)
            {
                GridPosition adjacentPosition = new GridPosition(currentPosition.x + direction.x, currentPosition.y + direction.y);

                if (gridSystem.CanPerformOnPosition(adjacentPosition))
                {
                    GridObject adjacentGridObject = gridSystem.GetGridObject(adjacentPosition);
                    UnitType adjacentUnitType = adjacentGridObject.GetUnit().GetUnitType();
                    if (adjacentUnitType == UnitType.Block)
                    {
                        BlockData adjacentUnitSO = adjacentGridObject.GetUnit().GetUnitData() as BlockData;
                        BlockColor adjacentColor = adjacentUnitSO.blockColor;

                        if (adjacentColor == startColor && !visited.Contains(adjacentPosition))
                        {
                            stack.Push(adjacentPosition);
                        }
                    }
                }
            }
        }

        return sameColorPositions;
    }

    public static Dictionary<int, int> FindEmptyCellsMap(GridSystem gridsystem)
    {
        Dictionary<int, int> columCountMap = new Dictionary<int, int>();
        int height = gridsystem.GetHeight();
        int width = gridsystem.GetWidth();

        for(int j =0;j<height; j++)
        {
            for(int i=0; i< width; i++)
            {
                GridPosition gridPosition = new GridPosition(i, j);
                if (!gridsystem.CanPerformOnPosition(gridPosition))
                {
                    if (columCountMap.ContainsKey(i))
                    {
                        columCountMap[i]++;
                    }
                    else
                    {
                        columCountMap[i] = 1;
                    }
                    
                }
            }
        }
        return columCountMap;
    }

    public static GridPosition FindLowestEmptyPositionBelow(GridSystem gridSystem,GridPosition startPosition)
    {
        int x = startPosition.x;
        int y = startPosition.y;
        y = Mathf.Min(y, gridSystem.GetHeight() + 1); //startPosition can be from hidden row

        while (y > 0)
        {
            GridPosition belowPosition = new GridPosition(x, y - 1);
            if (!gridSystem.CanPerformOnPosition(belowPosition))
            {
                y--;
            }
            else
            {
                break;
            }
        }
        if (y == gridSystem.GetHeight() + 1)
        {
            return new GridPosition(x, startPosition.y);
        }
        else
        {
            return new GridPosition(x, y);
        }

    }

}
