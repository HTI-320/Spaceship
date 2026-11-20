using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] asteroidPrefabs;
    public float spawnInterval = 2.5f;

    [Header("Distance Around Player")]
    public float minSpawnDistance = 40f;
    public float maxSpawnDistance = 70f;

    private Transform player;
    private float timer;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your spaceship has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null || asteroidPrefabs == null || asteroidPrefabs.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnAsteroid();
            timer = 0f;
        }
    }

    void SpawnAsteroid()
    {
        // Pick a random asteroid prefab from the array
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject prefabToSpawn = asteroidPrefabs[randomIndex];

        // Pick a random direction in 3D space and place it in a hollow shell around the player
        Vector3 randomDirection = Random.onUnitSphere;
        float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPosition = player.position + (randomDirection * spawnDistance);

        Instantiate(prefabToSpawn, spawnPosition, Random.rotation);
    }
}