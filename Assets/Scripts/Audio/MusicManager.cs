using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public FMOD.Studio.EventInstance musicInstance;
    public FMODUnity.EventReference fmodEvent;

    [Range(0f, 1f)]
    public float progress = 0f;

    [Range(0f, 1f)]
    public float volume = 1f;
    public string volumePath;

    public Genre playerGenre;
    public Genre enemyGenre;

    public bool musicRunning { get; private set; }

    private BeatSystem beatSystem;

    void Awake()
    {
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        beatSystem = GetComponent<BeatSystem>();
    }

    void Start()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        if (!musicInstance.isValid())
        {
            Debug.LogError("[MusicManager] EventInstance is invalid");
            return;
        }

        musicInstance.setParameterByName("parameter:/Music/Fight/Genre 1", (int)playerGenre);
        musicInstance.setParameterByName("parameter:/Music/Fight/Genre 2", (int)enemyGenre);
        musicInstance.setParameterByName("parameter:/Music/Fight/Progress", progress);
        SetGlobal(volumePath, volume);

        beatSystem.Init();

        musicInstance.start();
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    public void KillMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }

    void Update()
    {
        musicInstance.setParameterByName("parameter:/Music/Fight/Progress", progress);
        SetGlobal(volumePath, volume);
    }

    private void SetGlobal(string path, float val)
    {
        FMOD.RESULT result = FMODUnity.RuntimeManager.StudioSystem.setParameterByName(path, val);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to set parameter:" + result));
        }
    }

}
