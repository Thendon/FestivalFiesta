using System;
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
    public bool end = false;
    public int highscore =0;
    public bool menu = false;

    void Start()
    {
        text = scoreObject.GetComponent<TMPro.TMP_Text>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelState = FindObjectOfType<LevelState>();
        miniGame = FindObjectOfType<BeatMiniGame>();


        sceneLoader.onLevelChanged += onLevelChanged;
        sceneLoader.onLevelSuccess += onLevelSuccess;
        sceneLoader.onGameStart += resetScore;
    }

    void Update()
    {
        if (levelState != null)
        {
            Debug.Log(levelState.KilledEnemies);
            text.text = highscore.ToString();
            try
            {
                if (levelState.win && end == false)
                {
                    onLevelSuccess();
                    StartCoroutine(onWin());
                }
            }
            catch (Exception e)
            {

            }

            if (levelState.Progress <= 0 && menu ==false)
            {
                StartCoroutine(onLoose());
                menu = true;
            }
        }
    }

    public void resetScore()
    {
        highscore = 0;
        menu = false;
        //this.enabled = false;
    }

    public void onLevelChanged()
    {
        miniGame = FindObjectOfType<BeatMiniGame>();
        levelState = FindObjectOfType<LevelState>();
        miniGame.onHitMarker += addRanking;
        end = false;
    }
    
    public void onLevelSuccess()
    {
        highscore += levelState.KilledEnemies*5;
        end = true;
    }

    public IEnumerator onWin()
    {
  
        text.color = Color.red;
        yield return new WaitForSeconds(5);
        yield return new WaitForSeconds(5);
        text.color = Color.white;
        sceneLoader.LoadNextLevel();
    }

    public IEnumerator onLoose()
    {
        text.color = Color.red;
        yield return new WaitForSeconds(5);
        text.color = Color.white;
        sceneLoader.MainMenu();
    }

    public void addRanking(BeatMiniGame.HitRanking ranking, BeatSystem.MarkerType markerType)
    {

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