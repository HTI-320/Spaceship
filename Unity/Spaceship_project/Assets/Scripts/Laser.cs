using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 60f;
    public float lifetime = 3f;
    public int damage = 1;

    void Start()
    {
        // Automatically remove the laser after a few seconds if it misses
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            // Look for the Asteroid script on the hit object and apply damage
            if (other.TryGetComponent(out Asteroid asteroid))
            {
                asteroid.TakeDamage(damage);
            }

            Destroy(gameObject); // Always destroy the laser on hit
        }
    }
}