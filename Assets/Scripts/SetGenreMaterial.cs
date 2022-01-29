using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGenreMaterial : MonoBehaviour
{
    public Renderer[] renderers;

    public void SetMaterial(Material material)
    {
        if (renderers == null || renderers.Length < 1)
        {
            renderers = GetComponents<Renderer>();
        }
        foreach (var renderer in renderers)
            renderer.material = material;
    }

    public void SetMaterials(Material[] materials)
    {
        foreach (var renderer in renderers)
            renderer.material = materials[Random.Range(0, materials.Length)];
    }
}
