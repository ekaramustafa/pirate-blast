using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public static class Loader
{
    public enum Scene
    {
        MenuScene,
        LoadingScene,
        LevelScene,
    }

    private static Scene targetScene;
    public static void Load(Scene scene)
    {
        EventAggregator.GetInstance().ResetEventListeners();
        DOTween.KillAll();
        targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadTargetScene()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static Scene GetCurrentScene()
    {
        return targetScene;
    }

    public static void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public static void LoadLevel()
    {
        Load(Scene.LevelScene);
    }

    public static void LoadMenu()
    {
        Load(Scene.MenuScene);
    }

 
}