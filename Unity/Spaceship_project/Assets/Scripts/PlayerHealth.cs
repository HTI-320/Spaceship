using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject gameOverUI; // Optional: assign a UI panel later

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Spaceship Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            TakeDamage(1);
            Destroy(other.gameObject); // Destroy the asteroid on impact so it doesn't hit twice
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Disables movement, shooting, and hides the ship without breaking camera references
        gameObject.SetActive(false);
    }
}