using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusikPlayer : MonoBehaviour
{
    [SerializeField]
    AudioClip[] basePool = new AudioClip[0];
    [SerializeField]
    AudioSource audioSource = null;
    [SerializeField]
    bool autoPlayQueue = true;
    [SerializeField]
    bool autoAppendQueue = true;
    [SerializeField]
    bool randomizeBasePool = true;

    Queue<AudioClip> clipQueue = new Queue<AudioClip>();

    public AudioClip currentClip { get { return audioSource.clip; } }

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (autoAppendQueue)
            EnqueueBasePool();
    }

    void EnqueueBasePool()
    {
        AudioClip[] clips = basePool;

        if (randomizeBasePool)
        {
            AudioClip temp = null;
            for (int i = 0; i < clips.Length - 1; i++)
            {
                int rng = Random.Range(i, clips.Length);
                temp = clips[rng];
                clips[rng] = clips[i];
                clips[i] = temp;
            }
        }

        for (int i = 0; i < clips.Length; i++)
            clipQueue.Enqueue(clips[i]);
    }

    void Update()
    {
        if (autoPlayQueue && !audioSource.isPlaying)
        {
            if (clipQueue.Count == 0 && autoAppendQueue)
                EnqueueBasePool();
            if (clipQueue.Count > 0)
                PlayNextClip();
        }
    }

    public void EnqueueClip(AudioClip clip)
    {
        clipQueue.Enqueue(clip);
    }

    public void PlayNextClip()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = clipQueue.Dequeue();
        audioSource.Play();
    }
}
