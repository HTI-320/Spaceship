using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    public float fireCooldown = 0.2f;

    private float nextFireTime = 0f;

    void Update()
    {
        bool mouseFire = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool keyFire = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        bool joyFire = ArduinoInput.Instance != null && ArduinoInput.Instance.Fire2Down;
        // For continuous fire while the button is held, use .Fire2 instead of .Fire2Down

        if ((mouseFire || keyFire || joyFire) && Time.time >= nextFireTime)
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

            if (firePoint2 != null)
                Instantiate(laserPrefab, firePoint2.position, firePoint2.rotation);
        }
    }
}