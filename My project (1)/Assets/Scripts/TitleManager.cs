using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}