using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;

    [Header("점수 설정")]
    public int scorePerRock = 10;
    public int scorePerCorpse = 15;

    private int currentScore = 0;
    private int currentWave = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int rockCount, int corpseCount)
    {
        int earnedScore = (rockCount * scorePerRock) + (corpseCount * scorePerCorpse);
        currentScore += earnedScore;

        if (rockCount > 0) Debug.Log($"돌 흡수! +{earnedScore}점 / 총 점수: {currentScore}");
        if (corpseCount > 0) Debug.Log($"시체 흡수! +{earnedScore}점 / 총 점수: {currentScore}");

        UpdateUI();
    }

    public void UpdateWave(int wave)
    {
        currentWave = wave;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score : " + currentScore;

        if (waveText != null)
            waveText.text = "Wave : " + currentWave;
    }
}