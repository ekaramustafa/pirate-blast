using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelPlaySection : IEditorSection
{
    private LevelEdit levelEdit;

    private Func<List<string>> GetLevelOptionsList;
    private Func<int> GetSelectedLevelIndex;
    private Action<int> SetSelectedLevelIndex;
    private Action ResetPreferences;
    private Action UpdateLevelOptions;


    public LevelPlaySection(LevelEdit levelEdit, 
                            Func<List<string>> GetLevelOptionsList,
                            Func<int> GetSelectedLevelIndex,
                            Action<int> SetSelectedLevelIndex,
                            Action ResetPreferences,
                            Action UpdateLevelOptions
                            )
    {
        this.levelEdit = levelEdit;
        this.GetLevelOptionsList = GetLevelOptionsList;
        this.GetSelectedLevelIndex = GetSelectedLevelIndex;
        this.SetSelectedLevelIndex = SetSelectedLevelIndex;
        this.ResetPreferences = ResetPreferences;
        this.UpdateLevelOptions = UpdateLevelOptions;
    }


    public void Draw()
    {
        #region Level Selection
        int newSelectedLevelIndex = EditorGUILayout.Popup("Select Level", GetSelectedLevelIndex(), GetLevelOptionsList().ToArray());
        if (newSelectedLevelIndex != GetSelectedLevelIndex())
        {
            ResetPreferences();
            SetSelectedLevelIndex(newSelectedLevelIndex);
        }
        #endregion

        #region Play & Delete Buttons
        GUILayoutOption levelButtonWidth = GUILayout.MinWidth(150);
        GUILayoutOption levelButtonHeight = GUILayout.Height(30);
        GUILayoutOption expandingOption = GUILayout.ExpandWidth(true);

        Texture2D redTex = GetTextureForDeleteButton(150, 30, new Color(0.3f, 0.1f, 0));
        GUIStyle style = new GUIStyle();
        style.normal.background = redTex;
        style.margin = new RectOffset(4, 4, 2, 2);
        style.alignment = TextAnchor.MiddleCenter;


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        IEditorCommand playCommand = new EditorPlayCommand(levelEdit, () => GetSelectedLevelIndex());
        if (GUILayout.Button("Play the current level", levelButtonWidth, levelButtonHeight, expandingOption))
        {
            playCommand.Execute();
        }

        IEditorCommand deleteCommand = new EditorDeleteCommand(levelEdit, () => GetSelectedLevelIndex());
        if (GUILayout.Button("Delete the level", style, levelButtonWidth, levelButtonHeight, expandingOption))
        {
            deleteCommand.Execute();
            SetSelectedLevelIndex(0);
            UpdateLevelOptions();
        }
        #endregion

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private Texture2D GetTextureForDeleteButton(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        Texture2D backgroundTexture = new Texture2D(width, height);

        backgroundTexture.SetPixels(pixels);
        backgroundTexture.Apply();

        return backgroundTexture;
    }

}
