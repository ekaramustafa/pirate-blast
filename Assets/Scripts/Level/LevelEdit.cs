using System;
using System.Collections.Generic;
using UnityEngine;


public class LevelEdit : MonoBehaviour
{
    [Header("Assets and Sprites")]
    [SerializeField] private UnitAssetsData unitAssetsSO;
    [SerializeField] private SpriteRenderer blockFrame;

    private string blocksSortingLayerName = "Blocks";

    private LevelData levelData;
    private GridSystem gridSystem;


    private void Awake()
    {
        if (Application.isPlaying)
        {
            ResetGrid();
        }
    }

    #region Initialization Functions

    public bool LoadLevelDataForEditing(int level)
    {
        string levelDataFilePath = LevelDataLoaderWriter.GetLevelFilePath(level);
        levelData = LevelDataLoaderWriter.LoadLevelData(levelDataFilePath);

        if (levelData == null)
        {
            Debug.LogError("Failed to load level data.");
            return false;
        }
        return true;
    }

    public void InitializeGridSystem()
    {
        if (levelData != null)
        {
            if (IsGridInitialized())
            {
                ResetGrid();
                gridSystem = null;
            }
            gridSystem = new GridSystem(levelData, unitAssetsSO, blockFrame);
        }
    }
    #endregion


    #region Actions
    public void ResetGrid()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sortingLayerName == blocksSortingLayerName)
            {
                DestroyImmediate(obj);
            }
        }
        gridSystem = null;
    }

    public void SetUnitAtPosition(Vector3 mousePos, Sprite selectedSprite)
    {
        GridPosition gridPosition = GetGridPosition(mousePos);
        if (!gridSystem.CanPerformOnPosition(gridPosition)) return;

        UnitData unitSO = GetUnitSOBySprite(selectedSprite);
        UnitType unitType = unitSO.unitType;
        Unit unit = gridSystem.GetGridObject(gridPosition).GetUnit();
        unit.SetUnitSO(unitSO);

        gridSystem.GetUnitManager().AdjustPossibleUnitFormationsVisual();
        int index = gridPosition.y * gridSystem.GetWidth() + gridPosition.x;

        if (unitType == UnitType.Block)
        {
            BlockData blockSo = unitSO as BlockData;
            levelData.grid[index] = MappingUtils.blockColorToStringMapping[blockSo.blockColor];
        }
        else
        {
            levelData.grid[index] = MappingUtils.unitTypeToStringToMapping[unitSO.unitType];
        }

    }

    public void SaveGrid(int level) => LevelDataLoaderWriter.SaveLevelData(levelData, level);
    

    public void RemoveCurrentLevel(int level) => LevelDataLoaderWriter.DeleteLevel(level);

    public void CreateNewLevelWithGrid() => LevelDataLoaderWriter.CreateNewLevelData(levelData);

    #endregion


    #region Utility Functions
    public GridPosition GetGridPosition(Vector3 mousePos)
    {
        if(IsGridInitialized())
        {
            return gridSystem.GetGridPosition(mousePos);
        }
        else
        {
            throw new InvalidOperationException("Grid system is not initialized.");
        }
    }

    public bool IsGridInitialized()
    {
        return gridSystem != null;
    }

    

    private UnitData GetUnitSOBySprite(Sprite selectedSprite)
    {
        foreach (UnitAsset unitAsset in unitAssetsSO.unitAssets)
        {
            UnitData unitSO = unitAsset.unitData;
            if(unitSO.defaultStateSprite == selectedSprite)
            {
                return unitSO;
            }
        }
        return null;
    }

    public Dictionary<Sprite, string> GetSpriteMap()
    {
        Dictionary<Sprite, string> map = new Dictionary<Sprite, string>();
        foreach (UnitAsset unitAsset in unitAssetsSO.unitAssets)
        {
            Sprite key = unitAsset.unitData.defaultStateSprite;
            map[key] = UnitSOExtenstions.GetUnitName(unitAsset.unitData);
        }


        return map;
    }


    public string GetMoveCount()
    {
        return levelData.move_count.ToString();
    }

    public void SetMoveCount(int moveCount)
    {       
        levelData.move_count = moveCount;
    }
    #endregion

}
