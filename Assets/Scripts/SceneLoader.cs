using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    static SceneLoader instance = null;

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

    public Action onGameStart;
    public Action onMainMenu;
    public Action onLevelChanged;
    public Action onLevelSuccess;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);

        foreach (var palette in genrePalettes)
        {
            genrePalette.Add(palette.genre, palette);
        }
    }

    private void Start()
    {
        FindObjectOfType<MusicManager>().StartMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        if (scene.name != menuScene)
        {
            onLevelChanged?.Invoke();
            MusicManager musicManager = FindObjectOfType<MusicManager>();
            musicManager.playerGenre = selectedGenre;
            musicManager.enemyGenre = enemyGenre;
            musicManager.StartMusic();
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
        enemyGenre = gameSceneLoop.Dequeue();
        LoadScene(enemyGenre.ToString());
        gameSceneLoop.Enqueue(enemyGenre);
    }

    public void StartRound(string genre)
    {
        selectedGenre = (Genre)Enum.Parse(typeof(Genre), genre);
        onGameStart?.Invoke();

        gameSceneLoop.Clear();

        var scenes = Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList();
        Genre tempGO;
        for (int i = 0; i < scenes.Count - 1; i++)
        {
            int rnd = UnityEngine.Random.Range(i, scenes.Count);
            tempGO = scenes[rnd];
            scenes[rnd] = scenes[i];
            scenes[i] = tempGO;
        }

        foreach (Genre sceneGenre in scenes)
        {
            if (sceneGenre == selectedGenre)
                continue;

            gameSceneLoop.Enqueue(sceneGenre);
        }

        LoadNextLevel();
    }
}
