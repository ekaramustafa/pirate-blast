using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGrid : MonoBehaviour
{
    [Header("Assets and Sprites")]
    [SerializeField] private UnitAssetsData unitAssetsSO;
    [SerializeField] private SpriteRenderer blockFrame;
    
    private GridSystem gridSystem;
    private LevelData levelData;
    private ClickCommandInvoker clickCommandInvoker;

    private int totalGoals;
    private int goalsReached;
    private int moves;

    private bool isGameActive;

    private void Awake()
    {
        
        if (!LoadLevelData()) return;
        
        totalGoals = 0;
        goalsReached = 0;
        moves = levelData.move_count;
        EventAggregator.GetInstance().Subscribe<GoalsUpdateEvent>(OnGoalsUpdateEvent);
        isGameActive = true;
    }
    private void OnDestroy()
    {
        EventAggregator.GetInstance().Unsubscribe<GoalsUpdateEvent>(OnGoalsUpdateEvent);

    }

    private void Start()
    {
        
        gridSystem = new GridSystem(levelData, unitAssetsSO, blockFrame);
        clickCommandInvoker = new ClickCommandInvoker(gridSystem);

        //Publish UI Data
        PublishMoveWindowUIData();
        PublishGoalsWindowUIData();

    }

    private void Update()
    {
        if (!isGameActive) return;
        if (Input.GetMouseButtonDown(0))
        {
            OnClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void OnClick(Vector3 mousePos)
    {
        if (!isGameActive) return;
        isGameActive = false;

        bool performedAnyOperation = clickCommandInvoker.HandleClick(mousePos);
        if (!performedAnyOperation)
        {
            isGameActive = true;
            return;
        }

        if (moves != 0 && goalsReached < totalGoals)
        {
            isGameActive = true;
        }

        EventAggregator.GetInstance().Publish(new MoveConsumedEvent(1));
        moves--;

        if (moves == 0 && goalsReached < totalGoals)
        {
            FinishGame(false);
        }
    }

    private bool LoadLevelData()
    {
        int currentLevel = GameConstants.CurrentLevel;
        if(currentLevel > GameConstants.MAX_LEVEL)
        {
            Loader.LoadMenu();
            return false;
        }

        levelData = LevelDataLoaderWriter.LoadLevelData();
                if (levelData == null)
        {
            Debug.LogError("Failed to load level data.");
            return false;
        }
        return true;
    }

    public void PublishMoveWindowUIData()
    {
        EventAggregator.GetInstance().Publish(new MoveSetupEvent(levelData.move_count));
    }

    private void PublishGoalsWindowUIData()
    {
        Dictionary<Sprite, int> map = new Dictionary<Sprite, int>();

        string[] data_array = levelData.grid;

        foreach (string data in data_array)
        {
            UnitType unitType = MappingUtils.stringToUnitTypeMapping[data]();
            if (unitType == UnitType.Block || unitType == UnitType.TNT || unitType == UnitType.Random) continue;

            UnitData unitSO = unitAssetsSO.GetUnitSOByUnitType(unitType);


            if (unitSO != null)
            {
                Sprite sprite = unitSO.defaultStateSprite;

                if (sprite != null)
                {
                    if (map.ContainsKey(sprite))
                    {
                        map[sprite] += 1;
                    }
                    else
                    {
                        map[sprite] = 1;

                    }
                    totalGoals += 1;
                }
            }
        }
        EventAggregator.GetInstance().Publish(new GoalsSetupEvent(map));
    }

    private void OnGoalsUpdateEvent(GoalsUpdateEvent e)
    {
        
        foreach (KeyValuePair<Sprite, int> entry in e.parts)
        {
            goalsReached += entry.Value;
        }
        if (goalsReached >= totalGoals)
        {
            FinishGame(true);
        }
    }

    private void FinishGame(bool success)
    {
        isGameActive = false;
        EventAggregator.GetInstance().Publish(new LevelFinishedEvent(success));
    }



}
