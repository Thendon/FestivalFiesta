using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeatType = BeatSystem.BeatType;
using MarkerType = BeatSystem.MarkerType;

public class BeatMiniGame : MonoBehaviour
{
    enum HitRanking
    {
        Good,
        Medium,
        Bad
    }

    public float invisibleSize;
    // Raw delay to sync FMOD and Unity as we seem to have sync issues
    public float delayInSeconds;

    public RectTransform goodRegion;
    public RectTransform mediumRegion;
    public RectTransform badRegion;

    public BeatType currentBeatType;

    public GameObject tapMarkerPrefab;
    public GameObject regionMarkerPrefab;

    public float timeOffset;

    private Queue<RectTransform> activeMarkers = new Queue<RectTransform>();
    private Queue<RectTransform> inactiveMarkers = new Queue<RectTransform>();

    private RectTransform currentRegion;
    private float regionStartTime;

    private RectTransform rectTransform;

    private int markerIndex;

    private float speedPerSecond;

    private float delayOffset;

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

                        activeMarkers.Enqueue(markerTransform);
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

                        activeMarkers.Enqueue(markerTransform);
                        currentRegion = markerTransform;
                    }
                    break;
                case MarkerType.EndRegion:
                    {

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
        foreach (RectTransform marker in activeMarkers)
            marker.anchoredPosition += Vector2.left * Time.deltaTime * speedPerSecond;
        foreach (RectTransform marker in inactiveMarkers)
            marker.anchoredPosition += Vector2.left * Time.deltaTime * speedPerSecond;

        if (Input.GetButtonDown("Fire1"))
        {
            RectTransform marker = activeMarkers.Peek();

            if(marker.anchoredPosition.x < 0)
            {
                float distance = Mathf.Abs(-marker.anchoredPosition.x - (rectTransform.rect.width / 2.0f));

                bool hit = false;
                if (distance < goodRegion.rect.width / 2.0f)
                {
                    hit = true;

                    marker.GetComponent<Image>().color = Color.green;
                }
                else if (distance < mediumRegion.rect.width / 2.0f)
                {
                    hit = true;
                    marker.GetComponent<Image>().color = Color.yellow;
                }
                else if (distance < badRegion.rect.width / 2.0f)
                {
                    hit = true;
                    marker.GetComponent<Image>().color = Color.red;
                }

                if (hit)
                {
                    inactiveMarkers.Enqueue(activeMarkers.Dequeue());
                }

                //Debug.Break();
            }
        }

        

        int toDequeue = 0;
        foreach (RectTransform marker in activeMarkers)
        {
            if (marker.anchoredPosition.x <= -(rectTransform.rect.width / 2 + badRegion.rect.width / 2))
            {
                toDequeue++;
            }
        }

        for (int i = 0; i < toDequeue; i++)
        {
            RectTransform dequeuedMarker = activeMarkers.Dequeue();
            dequeuedMarker.GetComponent<Image>().color = Color.black;
            inactiveMarkers.Enqueue(dequeuedMarker);
        }

        toDequeue = 0;
        foreach (RectTransform marker in inactiveMarkers)
        {
            if(marker.anchoredPosition.x + marker.rect.width <= rectTransform.rect.x)
            {
                toDequeue++;
            }
        }

        for (int i = 0; i < toDequeue; i++)
        {
            GameObject toDelete = inactiveMarkers.Dequeue().gameObject;
            toDelete.name = "Delete";
            Destroy(toDelete.gameObject);
        }
    }
}
