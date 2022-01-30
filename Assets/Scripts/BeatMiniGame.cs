using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeatType = BeatSystem.BeatType;
using MarkerType = BeatSystem.MarkerType;

public class BeatMiniGame : MonoBehaviour
{
    public enum HitRanking
    {
        Good,
        Medium,
        Bad,
        NoHit
    }

    public float invisibleSize;
    // Raw delay to sync FMOD and Unity as we seem to have sync issues
    public float delayInSeconds;

    public RectTransform goodRegion;
    public RectTransform mediumRegion;
    public RectTransform badRegion;

    public BeatType currentBeatType;
    public MarkerType lastMarkerType;

    public GameObject tapMarkerPrefab;
    public GameObject regionMarkerPrefab;

    public float timeOffset;

    struct Marker
    {
        public RectTransform rect;
        public MarkerType type;
        public bool fireEndRegionEvent;
    }

    private Queue<Marker> activeMarkers = new Queue<Marker>();
    private Queue<Marker> inactiveMarkers = new Queue<Marker>();

    private RectTransform currentRegion;
    private float regionStartTime;

    private RectTransform rectTransform;

    private int markerIndex;

    private float speedPerSecond;

    private float delayOffset;

    public Action<HitRanking, MarkerType> onHitMarker;
    public Action onEndMarker;

    // Start is called before the first frame update
    void Start()
    {
        BeatSystem.markerEvent += OnNewMarkerCallback;
        rectTransform = transform as RectTransform;

        float distanceToTravel = Mathf.Abs(rectTransform.rect.width / 2 + invisibleSize);
        speedPerSecond = distanceToTravel * (1.0f / timeOffset);

        delayOffset = speedPerSecond * delayInSeconds;
    }

    void OnNewMarkerCallback(BeatType beatType, MarkerType markerType, float callbackTime)
    {
        Debug.Log("Hit new marker " + beatType + " " + markerType);

        if (beatType == currentBeatType)
        {
            switch (markerType)
            {
                case MarkerType.Tap:
                    {
                        GameObject newTapMarker = Instantiate(tapMarkerPrefab);
                        newTapMarker.transform.SetParent(transform);
                        RectTransform markerTransform = newTapMarker.transform as RectTransform;
                        markerTransform.anchorMin = new Vector2(1, 0.5f);
                        markerTransform.anchorMax = new Vector2(1, 0.5f);
                        float positionOffset = speedPerSecond * (Time.realtimeSinceStartup - callbackTime);
                        markerTransform.anchoredPosition = new Vector2(invisibleSize - positionOffset - delayOffset, 0);
                        newTapMarker.name = "Marker_" + markerIndex++;

                        Marker marker = new Marker();
                        marker.rect = markerTransform;
                        marker.type = MarkerType.Tap;
                        activeMarkers.Enqueue(marker);
                    }
                    break;
                case MarkerType.StartRegion:
                    {
                        GameObject newTapMarker = Instantiate(regionMarkerPrefab);
                        newTapMarker.transform.SetParent(transform);
                        RectTransform markerTransform = newTapMarker.transform as RectTransform;
                        markerTransform.anchorMin = new Vector2(1, 0.5f);
                        markerTransform.anchorMax = new Vector2(1, 0.5f);
                        float positionOffset = speedPerSecond * (Time.realtimeSinceStartup - callbackTime);
                        markerTransform.anchoredPosition = new Vector2(invisibleSize - positionOffset - delayOffset, 0);
                        markerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.0f);
                        newTapMarker.name = "Marker_" + markerIndex++;

                        Marker marker = new Marker();
                        marker.rect = markerTransform;
                        marker.type = MarkerType.StartRegion;
                        activeMarkers.Enqueue(marker);
                        currentRegion = markerTransform;
                    }
                    break;
                case MarkerType.EndRegion:
                    {
                        GameObject newTapMarker = Instantiate(regionMarkerPrefab);
                        newTapMarker.transform.SetParent(transform);
                        RectTransform markerTransform = newTapMarker.transform as RectTransform;
                        markerTransform.anchorMin = new Vector2(1, 0.5f);
                        markerTransform.anchorMax = new Vector2(1, 0.5f);
                        float positionOffset = speedPerSecond * (Time.realtimeSinceStartup - callbackTime);
                        markerTransform.anchoredPosition = new Vector2(invisibleSize - positionOffset - delayOffset, 0);
                        markerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.0f);
                        markerTransform.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                        Marker marker = new Marker();
                        marker.rect = markerTransform;
                        marker.type = MarkerType.EndRegion;
                        marker.fireEndRegionEvent = true;
                        activeMarkers.Enqueue(marker);
                        currentRegion = null;
                    }
                    break;
            }
        }
    }

    void Update()
    {
        if (currentRegion)
        {
            float newValue = currentRegion.rect.width + speedPerSecond * Time.deltaTime;

            currentRegion.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newValue);
        }

        // Update positions
        foreach (var marker in activeMarkers)
            marker.rect.anchoredPosition += Vector2.left * Time.deltaTime * speedPerSecond;
        foreach (var marker in inactiveMarkers)
            marker.rect.anchoredPosition += Vector2.left * Time.deltaTime * speedPerSecond;

        if ( activeMarkers.Count > 0)
        {
            Marker marker = activeMarkers.Peek();
            if (marker.type != MarkerType.EndRegion && Input.GetButtonDown("Fire1"))
            {
                if (marker.rect.anchoredPosition.x < 0)
                {
                    float distance = Mathf.Abs(-marker.rect.anchoredPosition.x - (rectTransform.rect.width / 2.0f));

                    bool hit = false;
                    if (distance < goodRegion.rect.width / 2.0f)
                    {
                        hit = true;
                        onHitMarker?.Invoke(HitRanking.Good, marker.type);

                        marker.rect.GetComponent<Image>().color = Color.green;
                    }
                    else if (distance < mediumRegion.rect.width / 2.0f)
                    {
                        hit = true;
                        onHitMarker?.Invoke(HitRanking.Medium, marker.type);
                        marker.rect.GetComponent<Image>().color = Color.yellow;
                    }
                    else if (distance < badRegion.rect.width / 2.0f)
                    {
                        hit = true;
                        onHitMarker?.Invoke(HitRanking.Bad, marker.type);
                        marker.rect.GetComponent<Image>().color = Color.red;
                    }
                    else
                    {
                        onHitMarker?.Invoke(HitRanking.NoHit, marker.type);
                    }

                    if (hit)
                    {
                        inactiveMarkers.Enqueue(activeMarkers.Dequeue());
                    }

                    //Debug.Break();
                }
            }
            else if (marker.type == MarkerType.EndRegion && marker.fireEndRegionEvent)
            {
                float distance = Mathf.Abs(-marker.rect.anchoredPosition.x - (rectTransform.rect.width / 2.0f));
                if (distance < goodRegion.rect.width / 2.0f)
                {
                    onEndMarker?.Invoke();
                    marker.fireEndRegionEvent = false;
                }
            }
        }        

        int toDequeue = 0;
        foreach (var marker in activeMarkers)
        {
            if (marker.rect.anchoredPosition.x <= -(rectTransform.rect.width / 2 + badRegion.rect.width / 2))
            {
                toDequeue++;
            }
        }

        for (int i = 0; i < toDequeue; i++)
        {
            var dequeuedMarker = activeMarkers.Dequeue();
            dequeuedMarker.rect.GetComponent<Image>().color = Color.black;
            inactiveMarkers.Enqueue(dequeuedMarker);
        }

        toDequeue = 0;
        foreach (var marker in inactiveMarkers)
        {
            if(marker.rect.anchoredPosition.x + marker.rect.rect.width <= rectTransform.rect.x)
            {
                toDequeue++;
            }
        }

        for (int i = 0; i < toDequeue; i++)
        {
            GameObject toDelete = inactiveMarkers.Dequeue().rect.gameObject;
            toDelete.name = "Delete";
            Destroy(toDelete.gameObject);
        }
    }
}
