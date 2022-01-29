using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    enum ControllerType
    {
        Keyboard,
        Gamepad
    }

    enum FireType
    {
        Projectile, // Spawns the projectile prefab at the given transform
        Beam, // Triggers an registerable event 
        Bomb
    }

    public uint maxPlayerLife = 10;
    // READONLY!!!
    [SerializeField]
    private uint currentPlayerLife;

    // In degrees per second
    public float carTurnSpeed = 90.0f;
    public float cannonTurnSpeed = 90.0f;
    // In m per second
    public float maxVelocity;

    // Rigid body test
    public float acceleration;

    // Used for cannon direction on PC
    public LayerMask environmentLayer;

    // Z must point forward
    public Transform cannonTransform;

    // Make sure this points towards forward direction
    public Transform weaponSpawnTransform;

    public GameObject projectilePrefab;

    public GameObject beamPrefab;

    public GameObject bombPrefab;

    private GameObject beamGameObject;

    private Rigidbody rigidBody;

    [SerializeField]
    private FireType currentFireType;

    private Vector3 wantedDirection;

    private ControllerType controllerType;

    public void ReceiveDamage(uint value)
    {
        if(currentPlayerLife <= value)
        {
            Debug.Log("Game over");
        }
        else
        {
            currentPlayerLife -= value;
            // TODO(Steffen): Trigger UI update
        }
    }

    void Start()
    {
        currentPlayerLife = maxPlayerLife;

        if(Input.GetJoystickNames().Length > 0)
        {
            controllerType = ControllerType.Gamepad;
        }
        else
        {
            controllerType = ControllerType.Keyboard;
        }

        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        wantedDirection.x = Input.GetAxis("Horizontal");
        wantedDirection.z = Input.GetAxis("Vertical");
        if(wantedDirection.sqrMagnitude > 0)
        {
            wantedDirection.Normalize();

            float changeValue = carTurnSpeed * Time.deltaTime;

            float signedAngle = Vector3.SignedAngle(transform.forward, wantedDirection, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            if (absAngle < changeValue)
            {
                changeValue = absAngle;
            }

            if (signedAngle < 0)
            {
                changeValue *= -1.0f;
            }

            if(Vector3.Magnitude(rigidBody.velocity) < maxVelocity)
            {
                rigidBody.AddForce(transform.forward * acceleration * Time.deltaTime, ForceMode.VelocityChange);
            }

            rigidBody.angularVelocity = new Vector3(0, changeValue, 0);
        }

        float cannonChangeValue = cannonTurnSpeed * Time.deltaTime;
        Vector3 aimDirection = Vector3.zero;
        switch (controllerType)
        {
            case ControllerType.Keyboard:
                {
                    RaycastHit hit;
                    Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(cameraRay, out hit, float.MaxValue, environmentLayer))
                    {
                        aimDirection = hit.point - transform.position;
                    }
                }
                break;
            case ControllerType.Gamepad:
                {
                    aimDirection.x = Input.GetAxis("RightStickHorizontal");
                    aimDirection.z = Input.GetAxis("RightStickVertical");
                }
                break;
            default:
                Debug.Assert(false, "Unsupported controller type");
                break;
        }

        if (aimDirection.sqrMagnitude > 0)
        {
            aimDirection.Normalize();
            float signedAngle = Vector3.SignedAngle(cannonTransform.forward, aimDirection, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            if (absAngle < cannonChangeValue)
            {
                cannonChangeValue = absAngle;
            }

            if (signedAngle < 0)
            {
                cannonChangeValue *= -1.0f;
            }

            cannonTransform.Rotate(Vector3.up, cannonChangeValue);
        }
        

        if (Input.GetButtonDown("Fire1"))
        {
            switch (currentFireType)
            {
                case FireType.Projectile:
                    {
                        GameObject projectileGameObject = Instantiate(projectilePrefab);
                        projectileGameObject.transform.position = weaponSpawnTransform.position;
                        projectileGameObject.transform.LookAt(weaponSpawnTransform.position + weaponSpawnTransform.forward, Vector3.up);
                    }
                    break;
                case FireType.Beam:
                    {
                        beamGameObject = Instantiate(beamPrefab);
                        beamGameObject.transform.position = weaponSpawnTransform.position;
                        beamGameObject.transform.LookAt(weaponSpawnTransform.position + weaponSpawnTransform.forward, Vector3.up);
                        beamGameObject.transform.SetParent(weaponSpawnTransform);
                    }
                    break;
                case FireType.Bomb:
                    {
                        GameObject bombGameObject = Instantiate(bombPrefab);
                        bombGameObject.transform.position = transform.position;
                    }
                    break;
                default:
                    Debug.Assert(false, "Need behaviour for unknown firetype: " + currentFireType);
                    break;
            }
            
        }
        else if (Input.GetButtonUp("Fire1") && currentFireType == FireType.Beam)
        {
            Destroy(beamGameObject);
        }
    }
}
