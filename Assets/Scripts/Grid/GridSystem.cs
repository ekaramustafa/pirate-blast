using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GridSystem
{
    private UnitManager unitManager;
    private RequestManager requestManager;

    private int width;
    private int height;
    private LevelData levelData;
    private UnitAssetsData unitAssetsSO;
    private GridObject[,] gridObjectsArray;

    private float horizantalCellSize;
    private float verticalCellSize;

    private float yBlockGeneratorWorldPosition;
    private float yWorldPositionOffset;
    private float xWorldPositionOffset;

    SpriteRenderer gridFrameOutlineSpriteRenderer;
    SpriteRenderer gridFrameBackgroundSpriteRenderer;



    public GridSystem(LevelData levelData, UnitAssetsData unitAssetsSO, SpriteRenderer gridFrameOutlineSpriteRenderer, SpriteRenderer gridFrameBackgroundSpriteRenderer)
    {
        this.levelData = levelData;
        this.width = levelData.grid_width;
        this.height = levelData.grid_height;
        this.unitAssetsSO = unitAssetsSO;
        this.gridFrameOutlineSpriteRenderer = gridFrameOutlineSpriteRenderer;
        this.gridFrameBackgroundSpriteRenderer = gridFrameBackgroundSpriteRenderer;

        this.horizantalCellSize = GameConstants.HORIZONTAL_CELL_SIZE;
        this.verticalCellSize = GameConstants.VERTICAL_CELL_SIZE;
        
        InitializeOffsets();
        InitializeGridObjects();

        requestManager = new RequestManager(width);
        
        unitManager = new UnitManager(this);
        unitManager.CreateUnits();
        InitializeFrame();

        //Play the animations unless we are editing the level
        if (Application.isPlaying)
        {
            AnimateBlocks();
            AnimateFrame();
            unitManager.AdjustPossibleUnitFormationsVisual();
        }

    }


    #region Initialization Functions
    private void InitializeOffsets()
    {
        float halfWidth = (width - 1) / 2f * horizantalCellSize;
        float halfHeight = height / 2f * verticalCellSize;

        xWorldPositionOffset = -halfWidth;
        yWorldPositionOffset = -halfHeight - GameConstants.TOP_BANNER_OFFSET;
        yBlockGeneratorWorldPosition = height - yWorldPositionOffset;

    }

    private void InitializeGridObjects()
    {

        gridObjectsArray = new GridObject[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GridPosition gridPosition = new GridPosition(i, j);
                GridObject gridObject = new GridObject(gridPosition);
                gridObjectsArray[i, j] = gridObject;
            }
        }
    }

    public void InitializeFrame()
    {
        GridPosition gridPosition = new GridPosition(width / 2, height / 2);
        Vector2 pos = GetWorldPosition(gridPosition);
        if(width % 2 == 0)
        {
            pos.x -= horizantalCellSize / 2;
        }
        if(height % 2 == 0)
        {
            pos.y -= verticalCellSize / 2;
        }
        gridFrameOutlineSpriteRenderer.transform.position = new Vector2(pos.x, pos.y);
        gridFrameOutlineSpriteRenderer.size = new Vector2(width * horizantalCellSize + GameConstants.UNIT_BACKGROUND_FRAME_HORIZANTAL_SIZE_ADDITION, height * verticalCellSize  + GameConstants.UNIT_BACKGROUND_FRAME_VERTICAL_SIZE_ADDITION);
        gridFrameBackgroundSpriteRenderer.transform.position = new Vector2(pos.x, pos.y);
        gridFrameBackgroundSpriteRenderer.size = new Vector2(width * horizantalCellSize + GameConstants.UNIT_BACKGROUND_FRAME_HORIZANTAL_SIZE_ADDITION, height * verticalCellSize + GameConstants.UNIT_BACKGROUND_FRAME_VERTICAL_SIZE_ADDITION);
    }

    #endregion


    #region Grid Initialization Animation Functions
    private void AnimateBlocks()
    {
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GridPosition gridPosition = new GridPosition(i, j);
                GridObject gridObject = GetGridObject(gridPosition);
                Unit unit = gridObject.GetUnit();
                Vector3 destination = unit.transform.position;
                animationService.TriggerAnimation(unit.transform, new Vector3(GameConstants.WIDTH, destination.y, destination.z), destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION, AnimationType.SLIDE);
            }
        }
    }

    private void AnimateFrame()
    {
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        Transform rendererTransform = gridFrameOutlineSpriteRenderer.transform;
        Vector3 destination = rendererTransform.position;
        animationService.TriggerAnimation(rendererTransform.transform, new Vector3(GameConstants.WIDTH, destination.y, destination.z), destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION, AnimationType.SLIDE);
        animationService.TriggerAnimation(gridFrameBackgroundSpriteRenderer.transform, new Vector3(GameConstants.WIDTH, destination.y, destination.z), destination, AnimationConstants.SLIDE_GAMESETUP_DEFAULT_DURATION, AnimationType.SLIDE);

    }
    #endregion

    #region Utility Functions

    public bool CanPerformOnPosition(GridPosition gridPosition)
    {
        return IsValidGridPosition(gridPosition) &&
            IsGridPositionFilled(gridPosition);
    }

    public bool CanDropToPosition(GridPosition gridPosition)
    {
        return IsValidGridPosition(gridPosition) && !IsGridPositionFilled(gridPosition)
             && !WillGridPositionBeOccupied(gridPosition);
    }

    public bool IsGridPositionInteractable(GridPosition position)
    {
        GridObject gridObject = GetGridObject(position);
        return gridObject.IsInteractable();
    }

    private bool IsGridPositionFilled(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridObject(gridPosition);
        return gridObject.GetUnit() != null;
    }

    private bool WillGridPositionBeOccupied(GridPosition gridPosition)
    {
        return GetGridObject(gridPosition).WillBeOccupied();
    }


    private bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
            gridPosition.x < width &&
            gridPosition.y >= 0 &&
            gridPosition.y < height;
    }
    public bool CanBeAffectedByNeighborBlast(GridPosition neighborPosition)
    {
        if (CanPerformOnPosition(neighborPosition))
        {
            Unit unit = GetGridObject(neighborPosition).GetUnit();
            return unit.CanBeAffectedByNeighborBlast();
        }
        return false;

    }



    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        Vector3 pos = new Vector3(gridPosition.x * horizantalCellSize, gridPosition.y * verticalCellSize);
        pos.x += xWorldPositionOffset;
        pos.y += yWorldPositionOffset;
        return pos;
    }


    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        worldPosition.x -= xWorldPositionOffset;
        worldPosition.y -= yWorldPositionOffset;
        return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / horizantalCellSize),
                Mathf.RoundToInt(worldPosition.y / verticalCellSize)
            );
    }

    public float GetWorldPositionX(int posX)
    {
        float worldPosX = posX * horizantalCellSize;
        worldPosX += xWorldPositionOffset;
        return worldPosX;
    }


    public UnitManager GetUnitManager() => unitManager;

    public RequestManager GetRequestManager() => requestManager;

    public int GetWidth() => width;
    public int GetHeight() => height;
    public UnitAssetsData GetUnitAssetsSO() => unitAssetsSO;
    public LevelData GetLevelData() => levelData;
    public float GetBlockGeneratorWorldPosition() => yBlockGeneratorWorldPosition;
    public GridObject GetGridObject(GridPosition gridPosition) => gridObjectsArray[gridPosition.x, gridPosition.y];


    #endregion

}
