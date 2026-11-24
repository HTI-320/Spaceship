using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 60f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}