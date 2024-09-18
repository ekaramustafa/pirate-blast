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

    public async UniTask DropUnits()
    {
        int width = gridSystem.GetWidth();
        int height = gridSystem.GetHeight();
        List<UniTask> moveTasks = new List<UniTask>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                GridPosition currentPosition = new GridPosition(x, y);
                GridObject currentGridObject = gridSystem.GetGridObject(currentPosition);

                if (!gridSystem.CanPerformOnPosition(currentPosition))
                    continue;

                UnitType unitType = currentGridObject.GetUnit().GetUnitType();
                if (unitType == UnitType.Stone || unitType == UnitType.Ice) continue;

                GridPosition lowestEmptyPosition = GridSearchUtils.FindLowestEmptyPositionBelow(gridSystem, currentPosition);
                if (lowestEmptyPosition != currentPosition && lowestEmptyPosition.y < height)
                {
                    Unit unit = currentGridObject.GetUnit();
                    UniTask moveTask = unit.MoveCoroutine(gridSystem.GetWorldPosition(lowestEmptyPosition), GameConstants.DROP_TIME).ToUniTask();
                    moveTasks.Add(moveTask);
                    MoveUnitToNewPosition(currentPosition, lowestEmptyPosition);
                }
            }
        }
        FillEmptyCells(moveTasks);
        await UniTask.WhenAll(moveTasks);
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

                GridPosition lowestEmptyPosition = GridSearchUtils.FindLowestEmptyPositionBelow(gridSystem, currentPosition);

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
