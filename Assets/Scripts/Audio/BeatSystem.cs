using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BeatSystem : MonoBehaviour
{
    public static event Action beatTickEvent;
    public static event Action<BeatType, MarkerType, float> markerEvent;

    public enum BeatType
    {
        Error,
        Melody,
        Drums
    }

    public enum MarkerType
    {
        Error,
        Tap,
        StartRegion,
        EndRegion
    }

    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
        public float lastTick = 0;
        public bool beatTickUpdate = false;
        
        public float beatMarkerTime;
        public bool beatMarkerUpdate;
        // NOTE(Steffen): There could be multiple markers on one frame, i.e. EndRegion and StartRegion
        public List<BeatType> beatTypes = new List<BeatType>();
        public List<MarkerType> markerTypes = new List<MarkerType>();
        public List<float> callbackTime = new List<float>();
        public FMOD.StringWrapper lastBeatMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    //public FMODUnity.EventReference eventName;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    /// <summary> Time of the last beat tick </summary>
    public float TimeSinceLastBeat
    {
        get { return Time.time - TimeOfLastBeat; }
        private set { TimeOfLastBeat = value; }
    }
    public float TimeOfLastBeat;
    
    /// <summary> Time until the next beat will tick </summary>
    public float TimeTillNextBeat
    {
        get { return (TimeOfLastBeat + 0.5f) - Time.time; }
    }

    /// <summary> Damage multiplier in Range [0,1] </summary>
    public float GetBeatTickDamageMultiplier
    {
        get
        {
            float minDist = Mathf.Min(TimeSinceLastBeat / 2f, TimeTillNextBeat / 2f);
            return Mathf.Clamp01(1f - minDist);
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        //eventName = FMODUnity.EventReference.Find("event:/Music/Level 01");
    }
#endif

    void Start() => Init();

    public void Init()
    {
        musicInstance = GetComponent<MusicManager>().musicInstance;

        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        //musicInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(
            beatCallback, 
            FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT 
            | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER
            //| FMOD.Studio.EVENT_CALLBACK_TYPE.NESTED_TIMELINE_BEAT
        );
        //musicInstance.start();
    }

    void OnDestroy()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("Current Bar = {0}, LastBeat = {2}, NextBeat = {3}, Last Marker = {1}", timelineInfo.currentMusicBar, (string)timelineInfo.lastMarker , timelineInfo.lastTick, TimeTillNextBeat));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                        timelineInfo.lastTick = Time.time;
                        timelineInfo.beatTickUpdate = true;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        string stringName = parameter.name;
                        BeatType beatType = BeatType.Error;
                        MarkerType markerType = MarkerType.Error;

                        if (stringName.StartsWith("StartRegion"))
                        {
                            markerType = MarkerType.StartRegion;
                        }
                        else if (stringName.StartsWith("EndRegion"))
                        {
                            markerType = MarkerType.EndRegion;
                        }
                        else if (stringName.StartsWith("Tap"))
                        {
                            markerType = MarkerType.Tap;
                        }

                        timelineInfo.beatMarkerUpdate = true;
                        timelineInfo.markerTypes.Add(markerType);
                        timelineInfo.beatTypes.Add(beatType);
                        timelineInfo.callbackTime.Add(Time.realtimeSinceStartup);
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.NESTED_TIMELINE_BEAT:
                    {
                        {
                            var parameter = (FMOD.Studio.TIMELINE_NESTED_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_NESTED_BEAT_PROPERTIES));
                            timelineInfo.currentMusicBar = parameter.properties.bar;
                        }
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }

    void Update()
    {
        if (timelineInfo.beatTickUpdate)
        {
            TimeOfLastBeat = timelineInfo.lastTick;
            beatTickEvent?.Invoke();
            timelineInfo.beatTickUpdate = false;
        }
        if (timelineInfo.beatMarkerUpdate)
        {
            for (int i = 0; i < timelineInfo.beatTypes.Count; i++)
            {
                BeatType beatType = timelineInfo.beatTypes[i];
                MarkerType markerType = timelineInfo.markerTypes[i];
                float callbackTime = timelineInfo.callbackTime[i];
                markerEvent?.Invoke(BeatType.Melody, markerType, callbackTime);
            }
            
            timelineInfo.beatMarkerUpdate = false;
            timelineInfo.beatTypes.Clear();
            timelineInfo.markerTypes.Clear();
            timelineInfo.callbackTime.Clear();
        }
    }
}