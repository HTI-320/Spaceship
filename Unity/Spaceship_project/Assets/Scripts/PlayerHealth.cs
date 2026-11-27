using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject gameOverUI;

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
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BlackHole"))
        {
            TakeDamage(maxHealth); // Instant Game Over
        }
    }

    void Die()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (Camera.main != null)
        {
            Camera.main.transform.SetParent(null);
        }

        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}