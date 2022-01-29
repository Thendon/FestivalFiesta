using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    string menuScene;
    [SerializeField]
    string gameScene;

    public string selectedGenre { get; private set; } = "";

    public void Exit()
    {
        print("Quit Game");
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    Scene currentScene;
    public Action onGameStart;
    public Action onMainMenu;

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene;
    }

    void LoadScene(string scenename)
    {
        print("Load Scene " + scenename);
        SceneManager.LoadScene(scenename);
    }

    public void MainMenu()
    {
        onMainMenu?.Invoke();
        LoadScene(menuScene);
    }

    public void StartRound(string genre)
    {
        selectedGenre = genre;
        onGameStart?.Invoke();
        LoadScene(gameScene);
    }
}
