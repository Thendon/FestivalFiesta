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
    [SerializeField]
    GenreMaterialPalette[] genrePalettes = new GenreMaterialPalette[0];

    public Dictionary<Genre, GenreMaterialPalette> genrePalette = new Dictionary<Genre, GenreMaterialPalette>();

    Queue<Genre> gameSceneLoop = new Queue<Genre>();

    public GenreMaterialPalette playerPalette { 
        get { 
            genrePalette.TryGetValue(selectedGenre, out GenreMaterialPalette palette);
            return palette; 
        } 
    }
    
    public GenreMaterialPalette enemyPalette
    {
        get
        {
            genrePalette.TryGetValue(enemyGenre, out GenreMaterialPalette palette);
            return palette;
        }
    }

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
    public Action onLevelChanged;
    public Action onLevelSuccess;

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);

        foreach (var palette in genrePalettes)
        {
            genrePalette.Add(palette.genre, palette);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene;
            Debug.Log(scene.name);
        if (scene.name != menuScene)
        {
        onLevelChanged?.Invoke();
        }
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
        //onLevelSuccess?.Invoke();
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
