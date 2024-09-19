using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class GameConstants
{

    #region Paths & Path Related Constants
    public const string LEVEL_FILES_PATH = "Levels";
    public const string LEVEL_FILE_PREFIX = "level_";
    #endregion

    #region
    private static int max_level;
    public static int MAX_LEVEL
    {
        get
        {
            max_level = Directory.GetFiles(Application.dataPath + "/" + GameConstants.LEVEL_FILES_PATH, "*.json").Length;
            return max_level;
        }
        set
        {
            max_level = value;
        }
    }
    #endregion

    #region Unit Constants
    /// <summary>
    /// The required number of adjacent blocks to perform blast
    /// </summary>
    public const int BLAST_THRESHOLD = 2;

    /// <summary>
    /// The required number of adjacent blocks to from tnt
    /// </summary>
    public const int TNT_FORMATION_BLOCKS_THRESHOLD = 5;

    /// <summary>
    /// The required number of adjacent tnts to from large tnt
    /// </summary>
    public const int TNT_COMBO_FORMATION_THRESHOLD = 2;

    /// <summary>
    /// Time for blocks to drop
    /// </summary>
    public const float DROP_TIME = 0.5f;
    #endregion


    #region Camera Constants
    public const int CAMERA_ORTHO_SIZE = 100;
    #endregion

    #region Grid Visual Constants
    public const float VERTICAL_CELL_SIZE = 11.5f;
    public const float HORIZONTAL_CELL_SIZE = 11.25f;

    public const int WIDTH = 1080;
    public const int HEIGHT = 1920;

    /// <summary>
    /// Experimental offset for top banner positioning
    /// </summary>
    public const int TOP_BANNER_OFFSET = 16;
   
    /// <summary>
    /// Experimental offset of the background for units
    /// </summary>
    public const float UNIT_BACKGROUND_FRAME_SIZE_ADDITION = 2f;

    #endregion

    #region Save System Constants
    /// <summary>
    /// Level Save Key for PlayerPrefs
    /// </summary>
    private const string CURRENT_LEVEL_KEY = "Current Level";
    public static int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
        }
        set
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value);
            PlayerPrefs.Save();
        }
    }
    #endregion

}
