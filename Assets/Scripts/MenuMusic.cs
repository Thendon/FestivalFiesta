using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [SerializeField]
    float speed = 0.1f;
    [SerializeField]
    GameObject progressBall;
    [SerializeField]
    float maxBallSize = 1.0f;
    [SerializeField]
    float maxBallDistance = 1.0f;

    public float value = 0.0f;

    MusicManager manager;
    AudioData audioData;

    private void Awake()
    {
        manager = GetComponent<MusicManager>();
        audioData = GetComponent<AudioData>();
    }

    Genre RandomGenre()
    {
        return (Genre)Random.Range(0, System.Enum.GetValues(typeof(Genre)).Length);
    }

    void Update()
    {
        value = Mathf.Cos(Time.time * speed);
        manager.progress = value * 0.5f + 0.5f;

        manager.playerGenre = RandomGenre();
        manager.enemyGenre = RandomGenre();
        while (manager.playerGenre == manager.enemyGenre)
            manager.enemyGenre = RandomGenre();

        progressBall.transform.localScale = Vector3.one * audioData.amplitudeBuffer * maxBallSize + Vector3.one * 0.0001f;
        Vector3 pos = progressBall.transform.localPosition;
        pos.x = value * maxBallDistance;
        progressBall.transform.localPosition = pos;
    }
}
