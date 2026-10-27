using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 어디서든 GameManager에 접근할 수 있게 하는 싱글톤 패턴
    public static GameManager Instance;

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;

    [Header("점수 설정")]
    public int scorePerRock = 10;     // 돌 1개당 점수
    public int scorePerCorpse = 15;   // 시체 1구당 점수 (15점으로 변경됨)

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

    // 이제 행성이 아이템을 먹을 때마다 즉시 이 함수가 호출됩니다.
    public void AddScore(int rockCount, int corpseCount)
    {
        int earnedScore = (rockCount * scorePerRock) + (corpseCount * scorePerCorpse);
        currentScore += earnedScore;

        if (rockCount > 0) Debug.Log($"돌 흡수! +{earnedScore}점 / 총 점수: {currentScore}");
        if (corpseCount > 0) Debug.Log($"시체 흡수! +{earnedScore}점 / 총 점수: {currentScore}");

        UpdateUI();
    }

    // 스포너에서 웨이브가 올라갈 때 이 함수를 부릅니다.
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