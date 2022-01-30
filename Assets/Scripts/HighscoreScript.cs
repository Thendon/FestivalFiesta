using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreScript : MonoBehaviour
{

    public LevelState levelState;
    private SceneLoader sceneLoader;
    public TMPro.TMP_Text text;
    public GameObject scoreObject;
    public BeatMiniGame miniGame;

    public int highscore =0;
    void Start()
    {
        text = scoreObject.GetComponent<TMPro.TMP_Text>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelState = FindObjectOfType<LevelState>();
        miniGame = FindObjectOfType<BeatMiniGame>();


        sceneLoader.onLevelChanged += onLevelChanged;
        sceneLoader.onLevelSuccess += onLevelSuccess;
    }

    void Update()
    {
        text.text = highscore.ToString();
        if (levelState.win)
        {
            onLevelSuccess();
        }
    }

    public void onLevelChanged()
    {
        miniGame = FindObjectOfType<BeatMiniGame>();
        levelState = FindObjectOfType<LevelState>();
        miniGame.onHitMarker += addRanking;
    }
    
    public void onLevelSuccess()
    {
        highscore += levelState.KilledEnemies*5;
        sceneLoader.LoadNextLevel();
    }

    public void addRanking(BeatMiniGame.HitRanking ranking, BeatSystem.MarkerType markerType)
    {
        Debug.Log("ranking erkannt");
        switch (ranking)
        {
            case BeatMiniGame.HitRanking.Good:      highscore += 5;
                break;
            case BeatMiniGame.HitRanking.Medium:    highscore += 3;
                break;
            case BeatMiniGame.HitRanking.Bad:       highscore += 1;
                break;
            case BeatMiniGame.HitRanking.NoHit:
 
                break;
        }

    }
}