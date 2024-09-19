using System;
using System.IO;
using UnityEngine;

public static class LevelDataLoaderWriter
{
    public static LevelData LoadLevelData(string levelFilePath)
    {
        if (File.Exists(levelFilePath))
        {
            string jsonContent = File.ReadAllText(levelFilePath);
            return JsonUtility.FromJson<LevelData>(jsonContent);
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
            return JsonUtility.FromJson<LevelData>(jsonContent);
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
        string json = JsonUtility.ToJson(levelData, true);
        string path = GetLevelFilePath(level);
        File.WriteAllText(path, json);
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
