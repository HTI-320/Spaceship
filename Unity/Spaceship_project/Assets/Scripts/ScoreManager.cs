using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;
    private int currentScore = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreUI(true);
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI(false);
    }

    void UpdateScoreUI(bool force)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }

        if (ArduinoInput.Instance != null)
            ArduinoInput.Instance.SendScore(currentScore, force);
    }
}