using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 60f;
    public float lifetime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid") || other.CompareTag("BlackHole"))
        {
            Asteroid target = other.GetComponentInParent<Asteroid>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}