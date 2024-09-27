using System;
using System.IO;
using UnityEngine;

public static class LevelDataLoaderWriter
{
    private static LevelData mlevelData;
    public static LevelData LoadLevelData(string levelFilePath)
    {
        if (File.Exists(levelFilePath))
        {
            string jsonContent = File.ReadAllText(levelFilePath);
            mlevelData = JsonUtility.FromJson<LevelData>(jsonContent);
            return mlevelData;
        }
        else
        {
            Debug.LogError("File not found at: " + levelFilePath);
            return null;
        }
    }

    public static LevelData LoadLevelData()
    {
        string levelFilePath = GetLevelFilePath(GameConstants.CurrentLevel);
        if (File.Exists(levelFilePath))
        {
            string jsonContent = File.ReadAllText(levelFilePath);
            mlevelData = JsonUtility.FromJson<LevelData>(jsonContent);
            return mlevelData;
        }
        else
        {
            Debug.LogError("File not found at: " + levelFilePath);
            return null;
        }
    }

    public static string GetLevelFilePath(int levelNumber)
    {
        string folderPath = Application.dataPath + "/" + GameConstants.LEVEL_FILES_PATH;
        string suffix = $"{levelNumber:D2}.json";
        return folderPath + "/" + GameConstants.LEVEL_FILE_PREFIX + suffix;
    }

    public static void SaveLevelData(LevelData levelData, int level)
    {
        UpdateGridArray(levelData);
        string json = JsonUtility.ToJson(levelData, true);
        string path = GetLevelFilePath(level);
        File.WriteAllText(path, json);
    }

    private static void UpdateGridArray(LevelData levelData)
    {
        if (levelData.grid_height == mlevelData.grid_height &&
            levelData.grid_width == mlevelData.grid_width)
        {
            return; // Height & Width are not modified
        }

        string[] new_grid = new string[levelData.grid_width * levelData.grid_height];

        int mHeight = mlevelData.grid_height;
        int mWidth = mlevelData.grid_width;
        string[] mGrid = mlevelData.grid;

        int minHeight = Math.Min(mHeight, levelData.grid_height);
        int minWidth = Math.Min(mWidth, levelData.grid_width);

        for (int j = 0; j < minHeight; j++)
        {
            for (int i = 0; i < minWidth; i++)
            {
                int oldIndex = j * mWidth + i;
                int newIndex = j * levelData.grid_width + i;
                new_grid[newIndex] = mGrid[oldIndex];
            }
        }

        for (int j = 0; j < levelData.grid_height; j++)
        {
            for (int i = 0; i < levelData.grid_width; i++)
            {
                int index = j * levelData.grid_width + i;

                if (new_grid[index] == null) // Only fill if it wasn't copied from the old grid
                {
                    new_grid[index] = "r"; // Put random block or any other default value
                }
            }
        }

        levelData.grid = new_grid;
    }

    public static void CreateNewLevelData(LevelData levelData)
    {
        string json = JsonUtility.ToJson(levelData, true);
        int lastLevel = GameConstants.MAX_LEVEL;
        string path = GetLevelFilePath(lastLevel + 1);
        File.WriteAllText(path, json);
        GameConstants.MAX_LEVEL += 1;

    }

    public static void DeleteLevel(int level)
    {
        string path = GetLevelFilePath(level);
        string meta_path = path + ".meta";
        GameConstants.MAX_LEVEL = GameConstants.MAX_LEVEL - 1;
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (File.Exists(meta_path))
        {
            File.Delete(meta_path);

        }

        for (int i = level + 1; i <= GameConstants.MAX_LEVEL + 1; i++)
        {
            string oldPath = GetLevelFilePath(i);
            string oldMetaPath = oldPath + ".meta";

            string newPath = GetLevelFilePath(i - 1);
            string newMetaPath = newPath + ".meta";

            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }

            if (File.Exists(oldMetaPath))
            {
                File.Move(oldMetaPath, newMetaPath);
            }
        }

    }


}
