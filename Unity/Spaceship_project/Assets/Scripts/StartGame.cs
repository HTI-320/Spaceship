using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject startMenuUI;

    void Start()
    {
        Time.timeScale = 0f;

        if (startMenuUI != null)
        {
            startMenuUI.SetActive(true);
        }
    }

    public void PlayGame()
    {
        if (startMenuUI != null)
        {
            startMenuUI.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}