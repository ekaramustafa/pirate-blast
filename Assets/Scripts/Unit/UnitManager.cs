using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitManager
{
    private GridSystem gridSystem;
    private UnitAssetsData unitAssetsData;
    private LevelData levelData;

    public UnitManager(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        unitAssetsData = gridSystem.GetUnitAssetsSO();
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
                    unitSO = unitAssetsData.GetBlockDataByBlockColor(blockColor);
                    if (unitSO == null)
                    {
                        Debug.LogError("CANNOT FIND THE BLOCK WITH GIVEN COLOR");
                        continue;
                    }
                }
                else if (unitType == UnitType.TNT)
                {
                    unitSO = unitAssetsData.GetTNTSOByTNTType(TNTType.NORMAL);
                    if (unitSO == null)
                    {
                        Debug.LogError("CANNOT FIND THE TNT WITH GIVEN TYPE");
                        continue;
                    }
                }
                else
                {
                    unitSO = unitAssetsData.GetUnitSOByUnitType(unitType);
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
        UnitData unitData = unitAssetsData.GetTNTSOByTNTType(TNTType.LARGE);
        CreateUnit(gridPosition, unitData);
    }

    public void CreateTNTUnit(GridPosition gridPosition)
    {
        UnitData unitData = unitAssetsData.GetUnitSOByUnitType(UnitType.TNT);
        CreateUnit(gridPosition, unitData);
    }

    public void CreateUnit(GridPosition gridPosition, UnitData unitData)
    {
        Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition);
        Transform unitTemplatePrefab = unitAssetsData.GetPrefabByUnitType(unitData.unitType);
        int sortingLayer = gridPosition.y;
        if(unitData.unitType == UnitType.TNT)
        {
            sortingLayer = 999;
        }

        Unit unit = UnitFactory.CreateUnit(unitData, unitTemplatePrefab, worldPosition, sortingLayer);
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetUnit(unit);
    }

    public async UniTask DropUnits(int startRow, int endRow, int startCol, int endCol)
    {
        DeActivateUnits(startRow, endRow, startCol, endCol);
        List<UniTask> moveTasks = new List<UniTask>();
        int startRowAdjusted = Mathf.Max(1, startRow); // exclude the first column, they cannot fall
        
        Dictionary<int, int> emptyColumnCountMaps = new Dictionary<int, int>();

        for (int x = startCol; x < endCol; x++)
        {
            for (int y = startRowAdjusted; y < endRow; y++)
            {
                GridPosition currentPosition = new GridPosition(x, y);

                if (!gridSystem.CanPerformOnPosition(currentPosition))
                    continue;

                Unit unit = gridSystem.GetGridObject(currentPosition).GetUnit();

                if (unit.IsStationary())
                    continue;

                UniTask moveTask = GetDropMoveTask(
                    unit, 
                    currentPosition,
                    (col, count) =>
                    {
                        emptyColumnCountMaps[col] = count;
                    } 
                );
                moveTasks.Add(moveTask);
            }
        }

        if(emptyColumnCountMaps.Count == 0)
        {
            for(int x= startCol; x < endCol; x++)
            {
                emptyColumnCountMaps[x] = endRow - startRow;
            }
        }
        FillEmptyCells(moveTasks, emptyColumnCountMaps);
        await UniTask.WhenAll(moveTasks);
        ActivateUnits(startRow, endRow, startCol, endCol);
    }



    public async UniTask DropUnits(UserRequest request)
    {
        int startCol = request.GetMinCol();
        int endCol = request.GetMaxCol() + 1;
        int startRow = request.GetMinRow();
        int endRow = gridSystem.GetHeight();
        await DropUnits(startRow,  endRow,  startCol, endCol);
    }


    public void SetUnitToPosition(Unit unit, GridPosition to)
    {
        GridObject toGridObject = gridSystem.GetGridObject(to);
        unit.SetSortingOrder(to.y);
        toGridObject.SetUnit(unit);
    }


    public void FillEmptyCells(List<UniTask> moveTasks, Dictionary<int, int> emptyColumnCountMaps)
    {
        int height = gridSystem.GetHeight();
        float yWorldPositionBlockGenerator = gridSystem.GetBlockGeneratorWorldPosition();
        Vector3 spawnWorldPosition = new Vector3(0, yWorldPositionBlockGenerator);
        GridPosition spawnGridPosition = new GridPosition();
        spawnGridPosition.y = height;
        
        foreach(KeyValuePair<int, int> pair in emptyColumnCountMaps)
        {
            spawnGridPosition.x = pair.Key;
            spawnWorldPosition.x = gridSystem.GetWorldPositionX(spawnGridPosition.x);
            int count = pair.Value;
            for(int i = 0; i < count; i++)
            {
                Unit unit = UnitFactory.CreateRandomBlockUnit(unitAssetsData, spawnWorldPosition, height);
                UniTask moveTask = GetDropMoveTask(unit, spawnGridPosition, 
                    (col, count) =>
                    {

                    });
                moveTasks.Add(moveTask);
            }
        }
    }

    public UniTask GetDropMoveTask(Unit unit, GridPosition startPosition, Action<int,int> callback)
    {
        Queue<GridPosition> moveQueue = new Queue<GridPosition>();
        GridPosition lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, startPosition);

        GridPosition posPtr = new GridPosition(startPosition.x, startPosition.y);
        while (lowerEmptyPosition != posPtr && lowerEmptyPosition.y < gridSystem.GetHeight() && lowerEmptyPosition.y >= 0)
        {
            moveQueue.Enqueue(lowerEmptyPosition);  // Queue all the moves first
            posPtr = lowerEmptyPosition;   // Update pointer
            lowerEmptyPosition = GridSearchUtils.FindLowerEmptyPositionBelow(gridSystem, posPtr);
        }

        UnitMover unitMover = unit.GetComponent<UnitMover>();

        List<Vector3> targetWorldPositions = new List<Vector3>();
        while (moveQueue.Count != 0)
        {
            targetWorldPositions.Add(gridSystem.GetWorldPosition(moveQueue.Dequeue()));
        }
        if (targetWorldPositions.Count == 0) return UniTask.CompletedTask;

        Vector3 lastPosition = targetWorldPositions[targetWorldPositions.Count - 1];
        gridSystem.GetGridObject(gridSystem.GetGridPosition(lastPosition)).SetWillBeOccupied(true);
        if (gridSystem.CanPerformOnPosition(startPosition))
        {
            gridSystem.GetGridObject(startPosition).SetUnit(null);
            //The start position might be the hidden position
        }

        Tween moveTask = unitMover.MoveWithDOTween(
            targetPositions: targetWorldPositions,
            totalTime: AnimationConstants.DROP_ANIMATION_DURATION,
            overshootAmount: AnimationConstants.DROP_ANIMATION_OVERSHOOT_AMOUNT,
            stepCallback: (Action<Unit, Vector3, Vector3>)((unit, sourceWorldPos, destinationWorldPos) =>
            {
                GridPosition sourcePos = gridSystem.GetGridPosition(sourceWorldPos);
                GridPosition destinationPos = gridSystem.GetGridPosition(destinationWorldPos);
                if(sourcePos.y < gridSystem.GetHeight())
                {
                    gridSystem.GetGridObject(sourcePos).SetUnit(null);
                }
                    
                SetUnitToPosition(unit, destinationPos);
            }),

            lastCallback: (targetWorldPos) =>
            {
                GridPosition targetPos = gridSystem.GetGridPosition(targetWorldPos);
                gridSystem.GetGridObject(targetPos).SetWillBeOccupied(false);
            }
        );
        callback?.Invoke(startPosition.x, targetWorldPositions.Count); //collecting information for fill empty positions

        return moveTask.AsyncWaitForCompletion().AsUniTask();
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
                        unit.SetSpriteToTNTState();
                    }
                }
                else
                {
                    Unit unit = gridObject.GetUnit();
                    unit.SetSpriteToDefault();
                }
            }
        }
    }






}
