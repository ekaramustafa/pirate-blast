using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                    unitSO = unitAssetsSO.GetBlockSOByBlockColor(blockColor);
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

    public void DropUnits(int startRow, int endRow, int startCol, int endCol)
    {
        DeActivateUnits(startRow, endRow, startCol, endCol);
        int height = gridSystem.GetHeight();
        List<UniTask> moveTasks = new List<UniTask>();

        for (int x = startCol; x < endCol; x++)
        {
            for (int y = startRow; y < endRow; y++)
            {
                GridPosition currentPosition = new GridPosition(x, y);
                GridObject currentGridObject = gridSystem.GetGridObject(currentPosition);

                if (!gridSystem.CanPerformOnPosition(currentPosition))
                    continue;

                UnitType unitType = currentGridObject.GetUnit().GetUnitType();
                if (unitType == UnitType.Stone || unitType == UnitType.Ice) continue;

                Queue<GridPosition> moveQueue = new Queue<GridPosition>();
                GridPosition lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, currentPosition);

                GridPosition posPtr = new GridPosition(currentPosition.x, currentPosition.y);
                while (lowerEmptyPosition != posPtr && lowerEmptyPosition.y < height && lowerEmptyPosition.y >= 0)
                {
                    moveQueue.Enqueue(lowerEmptyPosition);  // Queue all the moves first
                    posPtr = lowerEmptyPosition;   // Update current position
                    lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, posPtr);
                }

                // Now execute the queued moves
                while (moveQueue.Count > 0)
                {
                    GridPosition targetPosition = moveQueue.Dequeue();
                    Unit unit = gridSystem.GetGridObject(currentPosition).GetUnit();
                    UniTask moveTask = unit.MoveCoroutine(gridSystem.GetWorldPosition(targetPosition), GameConstants.DROP_TIME).ToUniTask();
                    moveTasks.Add(moveTask);
                    MoveUnitToNewPosition(currentPosition, targetPosition);  // Update the grid positions after moving
                    currentPosition = targetPosition;  // Update the current position after the move
                }
            }
        }
        FillEmptyCells(moveTasks);
        
        TaskScheduler.EnqueueTaskBatch(
            moveTasks,
            onStartCallback: () =>
            {
                Debug.Log("Drop Units Started");
            },
            onCompleteCallback: () =>
            {
                Debug.Log("Drop Units Completed");
                ActivateUnits(startRow, endRow, startCol, endCol);
            });
    }


    public void MoveUnitToNewPosition(GridPosition from, GridPosition to)
    {
        GridObject fromGridObject = gridSystem.GetGridObject(from);
        GridObject toGridObject = gridSystem.GetGridObject(to);
        Unit unit = fromGridObject.GetUnit();
        unit.SetSortingOrder(to.y);
        fromGridObject.SetUnit(null);

        toGridObject.SetUnit(unit);

    }

    public void FillEmptyCells(List<UniTask> moveTasks)
    {
        int height = gridSystem.GetHeight();
        float xWorldPositionOffsetBlockGenerator = gridSystem.GetBlockGeneratorOffset();
        Dictionary<int, int> emptyColumnCountMaps = GridSearchUtils.FindEmptyCellsMap(gridSystem);
        foreach (KeyValuePair<int, int> pair in emptyColumnCountMaps)
        {
            int key = pair.Key;
            int val = pair.Value;
            int adjustedY = gridSystem.GetGridPosition(new Vector3(0, xWorldPositionOffsetBlockGenerator, 0)).y;
            GridPosition currentPosition = new GridPosition(key, adjustedY);
            Vector3 worldPosition = gridSystem.GetWorldPosition(currentPosition);
            currentPosition.x = key;
            for (int i = 0; i < val; i++)
            {

                GridPosition lowestEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, currentPosition);

                if (lowestEmptyPosition != currentPosition && lowestEmptyPosition.y < height)
                {
                    BlockColor blockColor = BlockColorExtensions.GetRandomBlockColor();
                    UnitData unitSO = unitAssetsSO.GetBlockSOByBlockColor(blockColor);
                    Transform unitTemplatePrefab = unitAssetsSO.GetPrefabByUnitType(unitSO.unitType);
                    Unit unit = UnitFactory.CreateUnit(unitSO, unitTemplatePrefab, worldPosition, lowestEmptyPosition.y);
                    GridObject toGridObject = gridSystem.GetGridObject(lowestEmptyPosition);

                    UniTask moveTask = unit.MoveCoroutine(gridSystem.GetWorldPosition(lowestEmptyPosition), GameConstants.DROP_TIME).ToUniTask();
                    moveTasks.Add(moveTask);
                    toGridObject.SetUnit(unit);
                }
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
