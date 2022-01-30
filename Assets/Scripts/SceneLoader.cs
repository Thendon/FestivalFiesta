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
    string[] gameScenePool;

    Queue<Genre> gameSceneLoop = new Queue<Genre>();

    public Genre selectedGenre { get; private set; } = Genre.Metal;
    public Genre enemyGenre { get; private set; } = Genre.Schlager;

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
        gameSceneLoop.Clear();
    }

    public void LoadNextLevel()
    {
        enemyGenre = gameSceneLoop.Dequeue();
        LoadScene(enemyGenre.ToString());
        gameSceneLoop.Enqueue(enemyGenre);
    }

    public void StartRound(string genre)
    {
        selectedGenre = (Genre)Enum.Parse(typeof(Genre), genre);
        onGameStart?.Invoke();

        gameSceneLoop.Clear();

        foreach (Genre sceneGenre in Enum.GetValues(typeof(Genre)))
        {
            if (sceneGenre == selectedGenre)
                continue;

            gameSceneLoop.Enqueue(sceneGenre);
        }

        LoadNextLevel();
    }
}
