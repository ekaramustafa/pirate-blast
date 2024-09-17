using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDeleteCommand : IEditorCommand
{
    private LevelEdit levelEdit;
    private Func<int> getSelectedLevelIndex;

    public EditorDeleteCommand(LevelEdit levelEdit, Func<int> getSelectedLevelIndex)
    {
        this.levelEdit = levelEdit;
        this.getSelectedLevelIndex = getSelectedLevelIndex;

    }


    public void Execute()
    {
        levelEdit.RemoveCurrentLevel(getSelectedLevelIndex() + 1);
        levelEdit.ResetGrid();
    }
}
