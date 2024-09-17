using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditorSaveCommand : IEditorCommand
{
    private LevelEdit levelEdit;
    
    private Func<SaveOption> GetSelectedSaveOption;
    private Func<int> GetSelectedLevelIndex;
    private Func<string> GetMoveCount;

    private Action<int> SetSelectedLevelIndex;

    public EditorSaveCommand(
                LevelEdit levelEdit, 
                Func<SaveOption> getSelectedSaveOption, 
                Func<int> getSelectedLevelIndex, 
                Func<string> getMoveCount,
                Action<int> SetSelectedLevelIndex)
    {
        this.levelEdit = levelEdit;
        this.GetSelectedSaveOption = getSelectedSaveOption;
        this.GetSelectedLevelIndex = getSelectedLevelIndex;
        this.GetMoveCount = getMoveCount;
        this.SetSelectedLevelIndex = SetSelectedLevelIndex;
    }
    public void Execute()
    {
        int parsedValue;
        string moveCountString = GetMoveCount();
        if (!Int32.TryParse(moveCountString, out parsedValue))
        {
            throw new ArgumentException("Invalid value for move count: " + moveCountString);
        }

        levelEdit.SetMoveCount(parsedValue);
        if (GetSelectedSaveOption() == SaveOption.SaveCurrentLevel)
        {
            levelEdit.SaveGrid(GetSelectedLevelIndex() + 1);
        }
        else if (GetSelectedSaveOption() == SaveOption.CreateNewLevel)
        {
            levelEdit.CreateNewLevelWithGrid();
            SetSelectedLevelIndex(GameConstants.MAX_LEVEL - 1);
        }
    }


}

public enum SaveOption
{
    SaveCurrentLevel,
    CreateNewLevel
}
