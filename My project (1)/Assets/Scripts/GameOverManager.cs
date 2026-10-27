using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
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