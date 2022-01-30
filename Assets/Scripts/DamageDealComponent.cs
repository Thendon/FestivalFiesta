using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealComponent : MonoBehaviour
{
    public float baseDamage;
    public float damageMultiplier;

    public GameObject toDestroyOnCollision;

    private void OnTriggerEnter(Collider other)
    {
        Damagable d = other.GetComponent<Damagable>();
        if (d == null)
            other.GetComponentInParent<Damagable>();
        if (d == null)
            other.GetComponentInChildren<Damagable>();

        if (d != null)
        {
            d.GetDamage(baseDamage * damageMultiplier);
            if (toDestroyOnCollision != null)
                Destroy(toDestroyOnCollision);
        }
    }

}
