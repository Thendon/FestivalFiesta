using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStayDamage : MonoBehaviour
{
    public float baseDamage;
    public float damageMultiplier;

    private void OnTriggerStay(Collider other)
    {
        Damagable d = other.GetComponent<Damagable>();
        if (d == null)
            other.GetComponentInParent<Damagable>();
        if (d == null)
            other.GetComponentInChildren<Damagable>();

        if (d != null)
        {
            d.GetDamage(baseDamage * damageMultiplier * Time.deltaTime);
        }
    }
}
