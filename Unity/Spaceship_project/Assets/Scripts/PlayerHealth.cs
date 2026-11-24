using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject gameOverUI;
    public ArduinoController controller;

    private int currentHealth;
    private bool dead = false;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<ArduinoController>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        UpdateDisplay();

        // Arduino käynnistyy uudelleen, kun portti avataan, joten ensimmäinen
        // viesti voi mennä hukkaan. Siksi arvo lähetetään säännöllisesti.
        InvokeRepeating(nameof(UpdateDisplay), 1f, 1f);
    }

    public void TakeDamage(int damage)
    {
        if (dead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateDisplay();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateDisplay()
    {
        if (controller != null)
            controller.SendHealth(currentHealth);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject other)
    {
        if (other.CompareTag("Asteroid"))
        {
            TakeDamage(1);
            Destroy(other);
        }
    }

    void Die()
    {
        dead = true;
        CancelInvoke();

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        if (Camera.main != null)
            Camera.main.transform.SetParent(null);

        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}