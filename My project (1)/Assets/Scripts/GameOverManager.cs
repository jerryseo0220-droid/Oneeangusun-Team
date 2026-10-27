using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;

    }

    // 다시하기 → 시작 화면으로
    public void RestartGame()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }
}