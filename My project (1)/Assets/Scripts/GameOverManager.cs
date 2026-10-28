using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI bestScoreText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (finalScoreText != null)
        {
            finalScoreText.text = " : finalScoreText" + currentScore;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}