using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitManager
{
    private GridSystem gridSystem;
    private UnitAssetsData unitAssetsSO;
    private LevelData levelData;

    public UnitManager(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        unitAssetsSO = gridSystem.GetUnitAssetsSO();
        levelData = gridSystem.GetLevelData();
    }

    public void CreateUnits()
    {
        int height = gridSystem.GetHeight();
        int width = gridSystem.GetWidth();
        
        string[] data_array = levelData.grid;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {

                int index = j * width + i;
                string cell_value = data_array[index];
                UnitType unitType = MappingUtils.stringToUnitTypeMapping[cell_value]();
                UnitData unitSO;

                if (unitType == UnitType.Block)
                {
                    BlockColor blockColor = MappingUtils.stringToBlockColorMapping[cell_value]();
                    unitSO = unitAssetsSO.GetBlockDataByBlockColor(blockColor);
                    if (unitSO == null)
                    {
                        Debug.LogError("CANNOT FIND THE BLOCK WITH GIVEN COLOR");
                        continue;
                    }
                }
                else if (unitType == UnitType.TNT)
                {
                    unitSO = unitAssetsSO.GetTNTSOByTNTType(TNTType.NORMAL);
                    if (unitSO == null)
                    {
                        Debug.LogError("CANNOT FIND THE TNT WITH GIVEN TYPE");
                        continue;
                    }
                }
                else
                {
                    unitSO = unitAssetsSO.GetUnitSOByUnitType(unitType);
                }

                if (unitSO != null)
                {
                    GridPosition gridPosition = new GridPosition(i, j);
                    CreateUnit(gridPosition, unitSO);
                }

            }
        }
        AdjustPossibleUnitFormationsVisual();

    }

    public void DeActivateUnits(int startRow, int endRow, int startCol, int endCol)
    {
        for(int i = startCol; i < endCol; i++)
        {
            for(int j= startRow; j < endRow; j++)
            {
                GridObject gridObject = gridSystem.GetGridObject(new GridPosition(i, j));
                gridObject.SetIsInteractable(false);
            }
        }
    }

    public void ActivateUnits(int startRow, int endRow, int startCol, int endCol)
    {
        for (int i = startCol; i < endCol; i++)
        {
            for (int j = startRow; j < endRow; j++)
            {
                GridObject gridObject = gridSystem.GetGridObject(new GridPosition(i, j));
                gridObject.SetIsInteractable(true);
            }
        }
    }

    public void CreateComboTNTUnit(GridPosition gridPosition)
    {
        UnitData unitData = unitAssetsSO.GetTNTSOByTNTType(TNTType.LARGE);
        CreateUnit(gridPosition, unitData);
    }

    public void CreateTNTUnit(GridPosition gridPosition)
    {
        UnitData unitData = unitAssetsSO.GetUnitSOByUnitType(UnitType.TNT);
        CreateUnit(gridPosition, unitData);
    }

    public void CreateUnit(GridPosition gridPosition, UnitData unitData)
    {
        Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition);
        Transform unitTemplatePrefab = unitAssetsSO.GetPrefabByUnitType(unitData.unitType);
        Unit unit = UnitFactory.CreateUnit(unitData, unitTemplatePrefab, worldPosition, gridPosition.y);
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetUnit(unit);
    }

    public async UniTask DropUnits(int startRow, int endRow, int startCol, int endCol)
    {
        DeActivateUnits(startRow, endRow, startCol, endCol);
        List<UniTask> moveTasks = new List<UniTask>();

        // Iterate over the grid within the specified boundaries
        for (int x = startCol; x < endCol; x++)
        {
            for (int y = startRow; y < endRow; y++)
            {
                GridPosition currentPosition = new GridPosition(x, y);
                GridObject currentGridObject = gridSystem.GetGridObject(currentPosition);

                if (!gridSystem.CanPerformOnPosition(currentPosition))
                    continue;

                // Skip units that shouldn't move
                UnitType unitType = currentGridObject.GetUnit().GetUnitType();
                if (unitType == UnitType.Stone || unitType == UnitType.Ice)
                    continue;

                // Queue and perform all moves for this unit
                moveTasks.AddRange(QueueAndPerformMoves(currentPosition));
            }
        }

        // Fill empty cells while waiting for move operations
        FillEmptyCells(moveTasks);

        // Await all movement tasks
        await UniTask.WhenAll(moveTasks);
        ActivateUnits(startRow, endRow, startCol, endCol);
    }

    // Refactored function to handle movement and queuing logic
    private List<UniTask> QueueAndPerformMoves(GridPosition startPosition)
    {
        List<UniTask> moveTasks = new List<UniTask>();
        Queue<GridPosition> moveQueue = new Queue<GridPosition>();

        GridPosition lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, startPosition);
        GridPosition posPtr = startPosition;

        // Queue all valid moves
        while (lowerEmptyPosition != posPtr && lowerEmptyPosition.IsValid())
        {
            moveQueue.Enqueue(lowerEmptyPosition);
            posPtr = lowerEmptyPosition;
            lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, posPtr);
        }

        // Now perform the moves
        while (moveQueue.Count > 0)
        {
            GridPosition targetPosition = moveQueue.Dequeue();
            Unit unit = gridSystem.GetGridObject(startPosition).GetUnit();
            UniTask moveTask = unit.GetComponent<UnitMover>().MoveWithDOTween(
                gridSystem.GetWorldPosition(targetPosition), GameConstants.DROP_DURATION);

            moveTasks.Add(moveTask);
            MoveUnitToNewPosition(startPosition, targetPosition);
            startPosition = targetPosition;
        }

        return moveTasks;
    }

    // This method can remain unchanged but it's responsible for updating grid state after moves
    public void MoveUnitToNewPosition(GridPosition from, GridPosition to)
    {
        GridObject fromGridObject = gridSystem.GetGridObject(from);
        GridObject toGridObject = gridSystem.GetGridObject(to);
        Unit unit = fromGridObject.GetUnit();

        unit.SetSortingOrder(to.y);
        fromGridObject.SetUnit(null);
        toGridObject.SetUnit(unit);
    }

    // Refactored FillEmptyCells method
    public void FillEmptyCells(List<UniTask> moveTasks)
    {
        Dictionary<int, int> emptyColumnCountMaps = GridSearchUtils.FindEmptyCellsMap(gridSystem);
        foreach (var columnPair in emptyColumnCountMaps)
        {
            int column = columnPair.Key;
            int emptyCount = columnPair.Value;

            GridPosition spawnPosition = gridSystem.GetSpawnPositionForColumn(column);

            // For each empty cell in the column
            for (int i = 0; i < emptyCount; i++)
            {
                BlockColor blockColor = BlockColorExtensions.GetRandomBlockColor();
                UnitData unitData = unitAssetsSO.GetBlockDataByBlockColor(blockColor);
                Transform unitPrefab = unitAssetsSO.GetPrefabByUnitType(unitData.unitType);

                // Create a new unit and move it down to the empty spot
                GridPosition emptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, spawnPosition);
                Unit unit = UnitFactory.CreateUnit(unitData, unitPrefab, gridSystem.GetWorldPosition(spawnPosition), emptyPosition.y);

                gridSystem.GetGridObject(emptyPosition).SetUnit(unit);

                // Queue and perform move operations
                moveTasks.AddRange(QueueAndPerformMoves(emptyPosition));
            }
        }
    }
    public void AdjustPossibleUnitFormationsVisual()
    {
        int height = gridSystem.GetHeight();
        int width = gridSystem.GetWidth();
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                GridPosition startPosition = new GridPosition(i, j);
                if (!gridSystem.CanPerformOnPosition(startPosition)) continue;
                GridObject gridObject = gridSystem.GetGridObject(startPosition);
                UnitType unitType = gridObject.GetUnit().GetUnitType();
                if (unitType != UnitType.Block) continue;                
                List<GridPosition> formableGridPositions = GridSearchUtils.GetAdjacentSameColorBlocks(gridSystem, startPosition);
                if (formableGridPositions.Count >= GameConstants.TNT_FORMATION_BLOCKS_THRESHOLD)
                {
                    foreach (GridPosition position in formableGridPositions)
                    {
                        GridObject formableGridObject = gridSystem.GetGridObject(position);
                        Unit unit = formableGridObject.GetUnit();
                        BlockData blockSO = unit.GetUnitData() as BlockData;
                        unit.GetComponent<SpriteRenderer>().sprite = blockSO.tntStateSprite;
                    }
                }
                else
                {
                    Unit unit = gridObject.GetUnit();
                    unit.GetComponent<SpriteRenderer>().sprite = unit.GetDefaultSprite();
                }
            }
        }
    }





}
