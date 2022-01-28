using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Hier noch so ne komische Enum(?) Dings liste mit den verschiedenen musikstilen, die gespawnt werden?!

    public int innerRange;
    public int outerRange;
    public GameObject[] spawnables;
    public Transform player;
    public int spawnLimit;
    //public int minDistance;
    private Vector3 spawnCenter = Vector3.zero;
    private List<GameObject> spawned;
    private List<GameObject> spawnedInRange;

    void Start()
    {
        spawned = new List<GameObject>();
        spawnedInRange = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawned.Count!=0)
        {
            foreach (GameObject s in spawned)
            {
                float magnitude = Vector3.Magnitude(s.transform.position - player.position);
                if (magnitude < outerRange)
                {
                    spawnedInRange.Add(s);
                } else if (spawnedInRange.Contains(s))
                {
                    spawnedInRange.Remove(s);
                }
            }

        }

        if (spawnedInRange.Count < spawnLimit)
        {
            SpawnRandom();
        }
    }

    public void SpawnRandom()
    {
        GameObject spawnable = Instantiate(spawnables[Random.Range(0, spawnables.Length)]);
        Vector3 spawnPos = player.position;
        Vector3 spawnRot = new Vector3(0, Random.Range(0, 360), 0);
        spawnPos += Random.onUnitSphere * innerRange;
        spawnPos = new Vector3(spawnPos.x, 0, spawnPos.z);

        spawnable.transform.localPosition = spawnPos;
        spawnable.transform.localRotation = Quaternion.Euler(spawnRot);

        spawned.Add(spawnable);
    }

}
