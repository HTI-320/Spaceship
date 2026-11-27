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
        SendHealthToArduino(true);

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        SendHealthToArduino(false);

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
    }

    void SendHealthToArduino(bool force)
    {
        if (ArduinoInput.Instance != null)
            ArduinoInput.Instance.SendHealth(currentHealth, force);
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