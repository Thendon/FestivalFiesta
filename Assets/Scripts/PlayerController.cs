using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // In degrees per second
    public float carTurnSpeed = 90.0f;
    public float cannonTurnSpeed = 90.0f;
    // In m per second
    public float speed;

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

    [SerializeField]
    private FireType currentFireType;

    private Vector3 wantedDirection;

    private ControllerType controllerType;

    void Start()
    {
        if(Input.GetJoystickNames().Length > 0)
        {
            controllerType = ControllerType.Gamepad;
        }
        else
        {
            controllerType = ControllerType.Keyboard;
        }
    }

    //private bool

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Vector3 point = collision.GetContact(0).point;

    //}

    //private void OnCollisionExit(Collision collision)
    //{
        
    //}

    void Update()
    {
        wantedDirection.x = Input.GetAxis("Horizontal");
        wantedDirection.z = Input.GetAxis("Vertical");
        Debug.Log(wantedDirection);
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

            transform.Rotate(Vector3.up, changeValue);
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        switch (controllerType)
        {
            case ControllerType.Keyboard:
                {
                    RaycastHit hit;
                    Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(cameraRay, out hit, float.MaxValue, environmentLayer))
                    {
                        Vector3 cannonWantedDirection = hit.point - transform.position;
                        float changeValue = cannonTurnSpeed * Time.deltaTime;

                        float signedAngle = Vector3.SignedAngle(cannonTransform.forward, cannonWantedDirection, Vector3.up);
                        float absAngle = Mathf.Abs(signedAngle);

                        if (absAngle < changeValue)
                        {
                            changeValue = absAngle;
                        }

                        if (signedAngle < 0)
                        {
                            changeValue *= -1.0f;
                        }

                        cannonTransform.Rotate(Vector3.up, changeValue);
                    }
                }
                break;
            case ControllerType.Gamepad:
                break;
            default:
                break;
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
