using System;
using UnityEngine;
using UnityEditor;

public class GridOptionsSection : IEditorSection
{

    private LevelEdit levelEdit;


    private Func<int> GetSelectedLevelIndex;
    private Func<SaveOption> GetSelectedSaveOption;
    private Func<string> GetMoveCount;
    private Func<bool> IsEditDisabled;
    private Func<string> GetWidth;
    private Func<string> GetHeight;

    private Action ResetPreferences;
    private Action UpdateLevelOptions;
    private Action<string> SetMoveCount;
    private Action<string> SetWidth;
    private Action<string> SetHeight;
    private Action<int> SetSelectedLevelIndex;
    private Action<SaveOption> SetSelectedSaveOption;



    public GridOptionsSection(LevelEdit levelEdit,
                                Func<int> GetSelectedLevelIndex,
                                Func<SaveOption> GetSelectedSaveOption,
                                Func<string> GetMoveCount,
                                Func<bool> IsEditDisabled,
                                Func<string> GetWidth,
                                Func<string> GetHeight,
                                Action<string> SetMoveCount,
                                Action<int> SetSelectedLevelIndex,
                                Action ResetPreferences,
                                Action UpdateLevelOptions,
                                Action<SaveOption> SetSelectedSaveOption,
                                Action<string> SetWidth,
                                Action<string> SetHeight
                                )
    {
        this.levelEdit = levelEdit;
        this.GetSelectedLevelIndex = GetSelectedLevelIndex;
        this.GetSelectedSaveOption = GetSelectedSaveOption;
        this.GetMoveCount = GetMoveCount;
        this.IsEditDisabled = IsEditDisabled;
        this.GetWidth = GetWidth;
        this.GetHeight = GetHeight;
        this.SetMoveCount = SetMoveCount;
        this.ResetPreferences = ResetPreferences;
        this.UpdateLevelOptions = UpdateLevelOptions;
        this.SetSelectedSaveOption = SetSelectedSaveOption;
        this.SetSelectedLevelIndex = SetSelectedLevelIndex;
        this.SetWidth = SetWidth;
        this.SetHeight = SetHeight;

    }

    public void Draw()
    {
        GUILayout.Label("Grid Options");
        GUILayoutOption gridButtonWidth = GUILayout.MinWidth(150);
        GUILayoutOption gridButtonHeight = GUILayout.Height(30);
        GUILayoutOption expandingOption = GUILayout.ExpandWidth(true);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        #region Initialize Grid
        IEditorCommand initializeCommand = new EditorInitializeCommand(
            levelEdit,
            () => GetSelectedLevelIndex(),
            (new_value) => SetMoveCount(new_value),
            (new_value) => SetWidth(new_value),
            (new_value) => SetHeight(new_value)
            );
        if (GUILayout.Button("Initialize Grid", expandingOption, gridButtonWidth, gridButtonHeight))
        {
            initializeCommand.Execute();
            if (IsEditDisabled())
            {
                SceneVisibilityManager.instance.DisableAllPicking();
            }
        }
        #endregion
        GUILayout.FlexibleSpace();

        #region Save Grid
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        IEditorCommand saveCommand = new EditorSaveCommand(levelEdit,
            GetSelectedSaveOption: () => GetSelectedSaveOption(),
            GetSelectedLevelIndex: () => GetSelectedLevelIndex(),
            GetMoveCount: () => GetMoveCount(),
            GetWidth: () => GetWidth(),
            GetHeight: () => GetHeight(),
            (new_value) => SetSelectedLevelIndex(new_value)
            );

        if (GUILayout.Button("Save the Grid", expandingOption, gridButtonWidth, gridButtonHeight) && levelEdit.IsGridInitialized())
        {
            saveCommand.Execute();
            UpdateLevelOptions();

        }
        GUILayout.FlexibleSpace();
        SaveOption selectedOption = (SaveOption)GUILayout.Toolbar((int)GetSelectedSaveOption(), new string[] { "Current", "New" }, gridButtonWidth);
        SetSelectedSaveOption(selectedOption);
        GUILayout.EndVertical();
        #endregion

        GUILayout.FlexibleSpace();

        #region Reset the grid
        if (GUILayout.Button("Reset the Grid", expandingOption, gridButtonWidth, gridButtonHeight) && levelEdit.IsGridInitialized())
        {
            levelEdit.ResetGrid();
            ResetPreferences();
        }
        #endregion
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
