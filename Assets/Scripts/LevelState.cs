using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{

    public float Progress { get; private set; } = 0.1f;
    private float musicManagerProgress;
    public float progressRampUpSpeed = 1f;

    public bool win = false;

    public int KilledEnemies 
    {
        get { return _killedEnemies; }
    }

    private int _killedEnemies = 0;

    private MusicManager musicManager;

    private void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    void Update()
    {
        float i = Mathf.Clamp01(progressRampUpSpeed * Time.deltaTime);
        musicManagerProgress = Mathf.Lerp(musicManagerProgress, Progress, i);
        musicManager.progress = musicManagerProgress;

        if (Progress >=1 && win!=true)
        {
            win = true;
        }
    }

    public void EnemieDied()
    {
        _killedEnemies += 1;
        Progress += .01f;
    }

    public void PlayerGotDamage()
    {
        Progress -= .025f;
    }


}
