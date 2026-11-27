using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour
{
    public int health = 1;
    public int scoreValue = 1;
    public Image healthBarFill;

    private int maxHealth;

    void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = (float)health / maxHealth;
        }

        if (health <= 0)
        {
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue);
            }

            Destroy(gameObject);
        }
    }
}