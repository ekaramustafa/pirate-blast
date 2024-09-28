using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridUnitEditSection : IEditorSection
{

    private LevelEdit levelEdit;

    private Func<Sprite> GetSelectedSprite;
    private Func<string> GetMoveCount;
    private Func<string> GetWidth;
    private Func<string> GetHeight;
    private Func<bool> IsEditDisabled;
    
    private Action<bool> SetEditDisabled;
    private Action<Sprite> SetSelectedSprite;
    private Action<string> SetMoveCount;
    private Action<string> SetWidth;
    private Action<string> SetHeight;


    public GridUnitEditSection(
        LevelEdit levelEdit, 
        Func<string> GetMoveCount,
        Func<bool> IsEditDisabled,
        Func<Sprite> GetSelectedSprite,
        Func<string> GetWidth,
        Func<string> GetHeight,
        Action<bool> SetEditDisabled,
        Action<Sprite> SetSelectedSprite,
        Action<string> SetMoveCount,
        Action<string> SetWidth,
        Action<string> SetHeight
        )
    {
        this.levelEdit = levelEdit;
        this.GetMoveCount = GetMoveCount;
        this.GetSelectedSprite = GetSelectedSprite;
        this.GetWidth = GetWidth;
        this.GetHeight = GetHeight;
        this.IsEditDisabled = IsEditDisabled;
        this.SetEditDisabled = SetEditDisabled;
        this.SetSelectedSprite = SetSelectedSprite;
        this.SetMoveCount = SetMoveCount;
        this.SetWidth = SetWidth;
        this.SetHeight = SetHeight;

    }

    public void Draw()
    {
        GUILayout.Label("Editing Grid Units");
        if (!levelEdit.IsGridInitialized())
        {
            EditorGUILayout.HelpBox("Grid must be initialized before editing.", MessageType.Warning);
            return;
        }

        #region Edit Toggle
        bool newSelectionDisabled = EditorGUILayout.ToggleLeft("Enable Editing", IsEditDisabled());
        if (newSelectionDisabled != IsEditDisabled())
        {
            if (newSelectionDisabled)
            {
                SceneVisibilityManager.instance.DisableAllPicking();
            }
            else
            {
                SceneVisibilityManager.instance.EnableAllPicking();
            }
            SetEditDisabled(newSelectionDisabled);
        }

        if (!IsEditDisabled()) return;
        #endregion

        #region Move Count
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Move Count");
        string moveCount = GUILayout.TextField(GetMoveCount(), GUILayout.Width(100));
        SetMoveCount(moveCount);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        #endregion

        #region Width & Height
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginVertical();
        GUILayout.Label("Width");
        string width = GUILayout.TextField(GetWidth(), GUILayout.Width(100));
        SetWidth(width);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Height");
        string height = GUILayout.TextField(GetHeight(), GUILayout.Width(100));
        SetHeight(height);
        GUILayout.EndVertical();
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        #endregion

        #region Unit Selection
        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Unit Selection", EditorStyles.boldLabel);

        Sprite selectedSprite = (Sprite)EditorGUILayout.ObjectField("Select Unit", GetSelectedSprite(), typeof(Sprite), allowSceneObjects: false);
        SetSelectedSprite(selectedSprite);
        if (GetSelectedSprite() != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Selected Unit Preview:", EditorStyles.label);
            Rect spriteRect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(false));
            if (GetSelectedSprite().texture != null)
            {
                EditorGUI.DrawPreviewTexture(spriteRect, GetSelectedSprite().texture);
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DisplayUnitSelectionGrid();
        #endregion
    }

    private void DisplayUnitSelectionGrid()
    {
        GUILayout.Label("Select Unit", EditorStyles.boldLabel);
        int gridColumns = 3;
        int buttonSize = 80;
        Dictionary<Sprite, string> spriteMap = levelEdit.GetSpriteMap();
        List<Sprite> spriteOptions = spriteMap.Keys.ToList();


        if (spriteOptions == null || spriteOptions.Count == 0) return;

        int rowCount = Mathf.CeilToInt((float)spriteOptions.Count / gridColumns);

        for (int row = 0; row < rowCount; row++)
        {
            GUILayout.BeginHorizontal();

            for (int col = 0; col < gridColumns; col++)
            {
                int index = row * gridColumns + col;
                if (index >= spriteOptions.Count) break;

                Sprite sprite = spriteOptions[index];
                string label = spriteMap[sprite];
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label(label, GUILayout.Width(buttonSize));
                GUILayout.FlexibleSpace();
                if (sprite != null && sprite.texture != null)
                {
                    if (GUILayout.Button(sprite.texture, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        SetSelectedSprite(sprite);
                    }
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}
