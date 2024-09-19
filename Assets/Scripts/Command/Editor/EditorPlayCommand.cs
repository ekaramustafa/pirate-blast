using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorPlayCommand : IEditorCommand
{
    private Func<int> getSelectedLevelIndex;
    private LevelEdit levelEdit;

    public EditorPlayCommand(LevelEdit levelEdit, Func<int> selectedLevelIndex)
    {
        this.getSelectedLevelIndex = selectedLevelIndex;
        this.levelEdit = levelEdit;
    }


    public void Execute()
    {
        GameConstants.CurrentLevel = getSelectedLevelIndex() + 1;
        EditorApplication.isPlaying = true;
        
    }


}
