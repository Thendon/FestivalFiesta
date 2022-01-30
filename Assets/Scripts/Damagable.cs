using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public float Health;
    private float currentHealth;

    public Action onDeath;

    private void Awake()
    {
        currentHealth = Health;
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onDeath?.Invoke();

        Destroy(this.gameObject);
    }
}
