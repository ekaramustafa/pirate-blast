using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorInitializeCommand : IEditorCommand
{

    private LevelEdit levelEdit;
    private Func<int> getSelectedLevelIndex;
    private Action<string> setMoveCountString;

    public EditorInitializeCommand(LevelEdit levelEdit, Func<int> getSelectedLevelIndex, Action<string> setMoveCountString)
    {
        this.levelEdit = levelEdit;
        this.getSelectedLevelIndex = getSelectedLevelIndex;
        this. setMoveCountString = setMoveCountString;
    }

    public void Execute()
    {
        if (levelEdit.LoadLevelDataForEditing(getSelectedLevelIndex() + 1))
        {
            levelEdit.InitializeGridSystem();
            setMoveCountString(levelEdit.GetMoveCount());
        }
    }
}
    
