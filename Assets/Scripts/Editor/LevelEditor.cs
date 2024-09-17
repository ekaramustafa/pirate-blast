using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(LevelEdit))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    private string sortingLayerToBeHidden = "LevelCompleted";
    private LevelEdit levelEdit;

    //Edit variables
    private bool isEditDisabled;
    private Sprite selectedSprite;
    private List<string> levels = new List<string>();
    private int selectedLevelIndex = 0;
    private string moveCount;


    //Save variables
    private SaveOption selectedOption = SaveOption.SaveCurrentLevel;


    private LevelPlaySection levelPlaySection;
    private GridOptionsSection gridOptionsSection;
    private GridUnitEditSection gridUnitEditSection;

    private void OnEnable()
    {
        levelEdit = (LevelEdit)target;
        ResetPreferences();
        UpdateLevelOptions();
        ToggleCanvasVisibilityBySortingLayer();

        levelPlaySection = new LevelPlaySection(
            levelEdit: levelEdit,
            GetLevelOptionsList: () => GetLevelOptions(),
            GetSelectedLevelIndex: () => selectedLevelIndex,
            SetSelectedLevelIndex: (new_value) => selectedLevelIndex = new_value,
            ResetPreferences: () => ResetPreferences(),
            UpdateLevelOptions: () => UpdateLevelOptions()
            );

        gridOptionsSection = new GridOptionsSection(
            levelEdit: levelEdit,
            GetSelectedLevelIndex: () => selectedLevelIndex,
            GetSelectedSaveOption: () => selectedOption,
            GetMoveCount: () => moveCount,
            IsEditDisabled: () => isEditDisabled,
            SetMoveCount: (new_value) => moveCount = new_value,
            SetSelectedLevelIndex: (new_value) => selectedLevelIndex = new_value, 
            ResetPreferences: () => ResetPreferences(),
            UpdateLevelOptions: () => UpdateLevelOptions(),
            SetSelectedSaveOption : (new_value) => selectedOption = new_value
            );

        gridUnitEditSection = new GridUnitEditSection(
            levelEdit: levelEdit,
            GetMoveCount: () => moveCount,
            IsEditDisabled: () => isEditDisabled,
            GetSelectedSprite: () => selectedSprite,
            SetEditDisabled: (new_value) => isEditDisabled = new_value,
            SetSelectedSprite: (sprite) => selectedSprite = sprite,
            SetMoveCount: (new_value) => moveCount = new_value
            );

    }


    private void OnSceneGUI()
    {
        Event e = Event.current;

        if (isEditDisabled && e.type == EventType.MouseDown && e.button == 0 && selectedSprite)
        {
            levelEdit.SetUnitAtPosition(GetMousePosition(), selectedSprite);
        }

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying) return;
        GUILayout.Space(10);
        if (!InspectorIsLock())
        {
            EditorGUILayout.HelpBox("Level Editor inspector must be locked. Top right lock symbol in this tab.", MessageType.Warning);
            return;
        }
        levelPlaySection.Draw();
        GUILayout.Space(10);
        gridOptionsSection.Draw();
        GUILayout.Space(10);
        gridUnitEditSection.Draw();
        GUILayout.Space(10);

    }

    #region Utility Functions

    private void UpdateLevelOptions()
    {
        levels = new List<string>();
        for (int i = 0; i < GameConstants.MAX_LEVEL; i++)
        {
            levels.Add($"level {(i + 1).ToString()}");
        }
    }

    private List<string> GetLevelOptions()
    {
        levels = new List<string>();
        for (int i = 0; i < GameConstants.MAX_LEVEL; i++)
        {
            levels.Add($"level {(i + 1).ToString()}");
        }
        return levels;
    }

    private void ToggleCanvasVisibilityBySortingLayer()
    {
        int sortingLayerID = SortingLayer.NameToID(sortingLayerToBeHidden);

        Canvas[] canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            if (canvas.sortingLayerID == sortingLayerID)
            {
                SceneVisibilityManager.instance.Hide(canvas.gameObject, true);
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;
        float widthScale = currentWidth / GameConstants.WIDTH;
        float heightScale = currentHeight / GameConstants.HEIGHT;

        // Take the minimum of width and height scale to maintain aspect ratio
        float resolutionScale = Mathf.Min(widthScale, heightScale);
        Vector3 mousePosition = Event.current.mousePosition * 1.25f;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0f;

        return mousePosition;
    }

    private void ResetPreferences()
    {
        isEditDisabled = false;
        levelEdit.ResetGrid();
        selectedSprite = null;
        SceneVisibilityManager.instance.EnableAllPicking();
    }

    public static bool InspectorIsLock()
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(type);
        PropertyInfo info = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
        return (bool)info.GetValue(window, null);
    }
    #endregion




}
