using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorInitializeCommand : IEditorCommand
{

    private LevelEdit levelEdit;
    private Func<int> GetSelectedLevelIndex;
    private Action<string> SetMoveCountString;
    private Action<string> SetWidth;
    private Action<string> SetHeight;

    public EditorInitializeCommand(LevelEdit levelEdit, 
                    Func<int> GetSelectedLevelIndex, 
                    Action<string> SetMoveCountString,
                    Action<string> SetWidth,
                    Action<string> SetHeight
        )
    {
        this.levelEdit = levelEdit;
        this.GetSelectedLevelIndex = GetSelectedLevelIndex;
        this.SetMoveCountString = SetMoveCountString;
        this.SetWidth = SetWidth;
        this.SetHeight = SetHeight;
    }

    public void Execute()
    {
        if (levelEdit.LoadLevelDataForEditing(GetSelectedLevelIndex() + 1))
        {
            levelEdit.InitializeGridSystem();
            SetMoveCountString(levelEdit.GetMoveCount());
            SetWidth(levelEdit.GetCurrentLevelWidth());
            SetHeight(levelEdit.GetCurrentLevelHeight());
        }
    }
}
    
