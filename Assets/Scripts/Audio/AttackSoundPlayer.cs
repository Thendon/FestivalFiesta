using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSoundPlayer : MonoBehaviour
{

    string attackBase = "event:/Attack/";
    string eventString;

    private MusicManager musicPlayer;

    void Start()
    {
        musicPlayer = GetComponent<MusicManager>();

        eventString = attackBase + musicPlayer.playerGenre.ToString();

        PlayerController p = FindObjectOfType<PlayerController>();
        p.onGoodHit += PlayHit;
    }

    
    void PlayHit()
    {
        PlayOneShot(eventString);
    }

    public void PlayOneShot(string eventString)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventString);
    }
}
