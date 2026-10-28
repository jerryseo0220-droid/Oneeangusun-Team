using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class TitleManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI highScoreText; // 최고 점수를 띄울 텍스트

    private void Start()
    {
        // 게임 시작 시, 기기에 저장된 "HighScore" 값을 불러옵니다. (저장된 게 없으면 0)
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "HighScore : " + highScore ;
        }
    }

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