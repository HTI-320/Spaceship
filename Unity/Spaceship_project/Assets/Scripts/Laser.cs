using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 60f;
    public float lifetime = 3f;

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
            Destroy(other.gameObject); // Destroy the asteroid
            Destroy(gameObject);       // Destroy the laser
        }
    }
}