using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public FMOD.Studio.EventInstance instance;
    public FMODUnity.EventReference fmodEvent;

    [Range(0f, 1f)]
    public float progress;

    public Genre playerGenre;
    public Genre enemyGenre;

    void Awake()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
    }

    void Start()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        if (!instance.isValid())
        {
            Debug.LogError("[MusicManager] EventInstance is invalid");
            return;
        }

        instance.setParameterByName("parameter:/Music/Fight/Genre 1", (int)playerGenre);
        instance.setParameterByName("parameter:/Music/Fight/Genre 2", (int)enemyGenre);

        instance.start();
    }

    public void StopMusic()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void KillMusic()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void Update()
    {
        instance.setParameterByName("parameter:/Music/Fight/Progress", progress);
    }
}
