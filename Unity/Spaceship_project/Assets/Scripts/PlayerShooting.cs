using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public ArduinoController controller;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private float nextFireTime;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<ArduinoController>();
    }

    void Update()
    {
        if (controller == null) return;

        if (controller.FireInput && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (bulletPrefab != null && firePoint != null)
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}