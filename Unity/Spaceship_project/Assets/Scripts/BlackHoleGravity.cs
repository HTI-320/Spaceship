
using UnityEngine;

public class BlackHoleGravity : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Gravity")]
    public float influenceRadius = 100f;
    public float gravityStrength = 80f;
    public float maxPullSpeed = 60f;

    private Vector3 gravityVelocity;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 direction = transform.position - target.position;
        float distance = direction.magnitude;

        if (distance >= influenceRadius || distance < 0.1f)
            return;

        direction.Normalize();

        // Gravity becomes stronger closer to the black hole
        float influence = 1f - distance / influenceRadius;
        float acceleration = gravityStrength * influence * influence;

        gravityVelocity += direction * acceleration * Time.deltaTime;

        gravityVelocity = Vector3.ClampMagnitude(
            gravityVelocity,
            maxPullSpeed
        );

        // Pull spaceship without changing its rotation
        target.position += gravityVelocity * Time.deltaTime;
    }
}
