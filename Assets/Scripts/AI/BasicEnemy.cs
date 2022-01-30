using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BasicEnemy : MonoBehaviour
{
    public LayerMask meleeLayer;

    // How much damage this enemy deals on hit
    public uint damageValue = 1;

    public float turnSpeed;

    public float walkSpeed;

    public float acceleration;

    public PlayerController player;

    private Rigidbody rigidBody;

    private Animator animator;
    private float damageCooldown =0;
  
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
    }

    // Called by the animation event
    public void DealDamage()
    {
        player.ReceiveDamage(damageValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == meleeLayer)
        {
            animator.SetBool("InAttackRange", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == meleeLayer)
        {
            animator.SetBool("InAttackRange", false);
        }
    }

    void Update()
    {
        if (Vector3.Magnitude(player.transform.position - transform.position) > 17) 
                return;

        if (!animator.GetBool("InAttackRange"))
        {
            if (Vector3.Magnitude(rigidBody.velocity) < walkSpeed)
            {
                rigidBody.AddForce(transform.forward * acceleration * Time.deltaTime, ForceMode.VelocityChange);
            }
        }

        Vector3 directionToTarget = player.transform.position - transform.position;
        float currentDistance = Vector3.SqrMagnitude(directionToTarget);

        directionToTarget.y = 0.0f;
        directionToTarget.Normalize();

        float changeValue = turnSpeed * Time.deltaTime;

        float signedAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        float absAngle = Mathf.Abs(signedAngle);

        if (absAngle < changeValue)
        {
            changeValue = absAngle;
        }

        if (signedAngle < 0)
        {
            changeValue *= -1.0f;
        }

        rigidBody.angularVelocity = new Vector3(0, changeValue, 0);

        if(currentDistance < 3f && damageCooldown <= 0)
        {
            DealDamage();
            damageCooldown = 0.3f;
        }

        if(damageCooldown>0)
        damageCooldown -= Time.deltaTime;


    }

}
