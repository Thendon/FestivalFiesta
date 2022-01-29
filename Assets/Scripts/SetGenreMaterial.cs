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
}
