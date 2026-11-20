using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("LCD Display")]
    [SerializeField] private ArduinoController arduinoController;

    [Header("Shooting Settings")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.2f;

    private float nextFireTime = 0f;
    private int shotsFired;

    void Awake()
    {
        if (arduinoController == null)
            arduinoController = FindFirstObjectByType<ArduinoController>();
    }

    void Update()
    {
        bool mouseFire = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool keyFire = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if ((mouseFire || keyFire) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Shoot()
    {
        if (laserPrefab != null && firePoint != null)
        {
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        }

        shotsFired++;
        arduinoController?.SendToLCD("Shots: " + shotsFired);
    }
}