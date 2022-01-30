using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WeightedMaterial
{
    public Material material;
    [Range(0,100)]
    public int weighting;
}

[CreateAssetMenu(fileName = "GenreMaterialPalette", menuName = "ScriptableObjects/GenreMaterialPalette", order = 1)]
public class GenreMaterialPalette : ScriptableObject
{
    public Genre genre;
    public WeightedMaterial[] weightedMaterials;
    [ColorUsage(true, true)]
    public Color color;
    public Texture2D particleTex;

    public Material GetRandomMaterial()
    {
        if (weightedMaterials == null || weightedMaterials.Length < 1)
        {
            Debug.LogError("No Materials assigned to GenreMaterialPalette");
            return null;
        }

        int randomIndex = GetRandomWeightedIndex( weightedMaterials.Select(m => m.weighting).ToArray() );
        return weightedMaterials[randomIndex].material;
    }

    public int GetRandomWeightedIndex(int[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }

}
