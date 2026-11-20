using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float tumbleSpeed = 40f;

    private Transform player;

    void Start()
    {
        // Automatically finds the Player at runtime (essential for spawned Prefabs)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player ship has the 'Player' tag.");
        }
    }

    void Update()
    {
        // Slowly rotate the asteroid for visual effect
        transform.Rotate(new Vector3(1f, 0.5f, 0f) * tumbleSpeed * Time.deltaTime);

        // Move directly toward the player's current position
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
}