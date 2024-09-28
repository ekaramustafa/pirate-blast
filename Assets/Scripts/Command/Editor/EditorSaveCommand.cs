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
    private Func<string> GetWidth;
    private Func<string> GetHeight;

    private Action<int> SetSelectedLevelIndex;

    public EditorSaveCommand(
                LevelEdit levelEdit, 
                Func<SaveOption> GetSelectedSaveOption, 
                Func<int> GetSelectedLevelIndex, 
                Func<string> GetMoveCount,
                Func<string> GetWidth,
                Func<string> GetHeight,
                Action<int> SetSelectedLevelIndex
                )
    {
        this.levelEdit = levelEdit;
        this.GetSelectedSaveOption = GetSelectedSaveOption;
        this.GetSelectedLevelIndex = GetSelectedLevelIndex;
        this.GetMoveCount = GetMoveCount;
        this.GetWidth = GetWidth;
        this.GetHeight = GetHeight;
        this.SetSelectedLevelIndex = SetSelectedLevelIndex;
    }
    public void Execute()
    {
        int parsedMoveCount;
        string moveCountString = GetMoveCount();
        if (!Int32.TryParse(moveCountString, out parsedMoveCount))
        {
            throw new ArgumentException("Invalid value for move count: " + moveCountString);
        }
        int parsedWidth;
        string widthString = GetWidth();
        if (!Int32.TryParse(widthString, out parsedWidth))
        {
            throw new ArgumentException("Invalid value for width: " + widthString);
        }

        int parsedHeight;
        string heightString = GetHeight();
        if (!Int32.TryParse(heightString, out parsedHeight))
        {
            throw new ArgumentException("Invalid value for width: " + heightString);
        }

        levelEdit.SetMoveCount(parsedMoveCount);
        levelEdit.SetHeight(parsedHeight);
        levelEdit.SetWidth(parsedWidth);
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
