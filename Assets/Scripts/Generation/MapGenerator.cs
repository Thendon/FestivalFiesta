using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Hier noch so ne komische Enum(?) Dings liste mit den verschiedenen musikstilen, die gespawnt werden?!
    public Genre gegnerGenre = Genre.Schlager;
    public GenreMaterialPalette[] genreMaterialPalettes;
    public Dictionary<Genre, GenreMaterialPalette> genreMaterials;

    public Vector2 regionSize = Vector2.one;
    public Dictionary<Vector2Int, bool> regionGenerated;
    public Vector2Int GetCurrentRegion
    {
        get { 
            return new Vector2Int(
                (int)((player.transform.position.x / regionSize.x)), 
                (int)((player.transform.position.z / regionSize.y))
             ); 
        }
    }
    public Vector2Int currentRegion;

    public float radius = 1;
    public int rejectionSamples = 30;
    public float displayRadius = 1;

    List<Vector2> points = new List<Vector2>();

    public int innerRange;
    public int outerRange;
    public GameObject[] spawnables;
    public Transform player;
    public int spawnLimit;
    //public int minDistance;
    private Vector3 spawnCenter = Vector3.zero;
    private List<GameObject> spawned;
    private List<GameObject> spawnedInRange;


    private void Awake()
    {
        regionGenerated = new Dictionary<Vector2Int, bool>();
        genreMaterials = new Dictionary<Genre, GenreMaterialPalette>();
        foreach (var matPalette in genreMaterialPalettes)
            genreMaterials.Add(matPalette.genre, matPalette);
    }

    void Start()
    {
        spawned = new List<GameObject>();
        spawnedInRange = new List<GameObject>();
    }

    void Update()
    {
        currentRegion = GetCurrentRegion;
        GenerateNeighboringRegions(currentRegion);

        /*
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
        */
    }

    public void GenerateNeighboringRegions(Vector2Int region)
    {
        /*
        if (!regionGenerated.ContainsKey(region))
        {
            SpawnRegion(region);
        }
        */

        for (int x = -2; x <= 1; x++)
        {
            for (int y = -2; y <= 1; y++)
            {
                Vector2Int testRegion = new Vector2Int(region.x + x, region.y + y);
                if (!regionGenerated.ContainsKey(testRegion)){
                    SpawnRegion(testRegion);
                }
            }
        } 
    }

    public void SpawnRegion(Vector2Int regionCoords)
    {
        Debug.Log("Spawn Region: " + regionCoords);
        regionGenerated.Add(regionCoords, true);

        var addPoints = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
        SpawnObjects(regionCoords, addPoints);

        foreach (var p in addPoints)
        {
            points.Add(p + new Vector2(regionCoords.x * regionSize.x, regionCoords.y * regionSize.y));
        }
    }

    public void SpawnObjects(Vector2Int regionCoords, List<Vector2> positions)
    {
        var region = new GameObject("Region" + regionCoords.ToString());
        region.transform.parent = this.transform;
        Vector2 regionPos = new Vector2(regionCoords.x * regionSize.x, regionCoords.y * regionSize.y);
        region.transform.position = new Vector3(regionPos.x, 0, regionPos.y);

        foreach (var p in positions)
        {
            Vector2 position = p + regionPos;   
            var inst = SpawnRandom(new Vector3(position.x,0, position.y));
            inst.transform.parent = region.transform;

            points.Add(position);
        }

    }


    public GameObject SpawnRandom(Vector3 position)
    {
        GameObject spawnable = Instantiate(spawnables[Random.Range(0, spawnables.Length)]);
        Vector3 spawnRot = new Vector3(0, Random.Range(0, 360), 0);

        spawnable.transform.position = position;
        spawnable.transform.localRotation = Quaternion.Euler(spawnRot);

        spawned.Add(spawnable);

        // random material pro gegnergenre
        var setGenreMat = spawnable.GetComponent<SetGenreMaterial>();
        if (setGenreMat != null)
        {
            Material mat = genreMaterials[gegnerGenre].GetRandomMaterial();
            if (mat != null) setGenreMat.SetMaterial(mat);
            else Debug.LogWarning("[MapGen] No Material for Genre");
        }

        return spawnable;
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(regionSize / 2, regionSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(currentRegion.x * regionSize.x, 0, currentRegion.y * regionSize.y), 1f);

        if (points != null)
        {
            Gizmos.color = Color.white;
            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), displayRadius);
            }
        }
    }

}
