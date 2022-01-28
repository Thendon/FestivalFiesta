using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedMaterial
{
    public Material material;
    [Range(0,999)]
    public int weighting;
}

public class GenreMaterialPalette : ScriptableObject
{
    public Genre genre;
    public WeightedMaterial[] weightedMaterials;
}
