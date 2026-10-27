using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHp = 100;
    public int currentHp;

    [Header("UI")]
    public Slider hpSlider;

    public bool IsDead { get; private set; }

    void Start()
    {
        currentHp = maxHp;
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        // 이미 죽었으면 데미지 무시
        if (IsDead)
            return;

        currentHp -= damage;

        if (currentHp < 0)
            currentHp = 0;

        Debug.Log(name + " HP: " + currentHp);

        UpdateBar();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void UpdateBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value =
                (float)currentHp / maxHp;
        }
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        // =========================
        // 플레이어 사망
        // =========================
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");

            // 혹시 게임이 정지되어 있을 경우 복구
            Time.timeScale = 1f;

            // 게임오버 화면으로 이동
            SceneManager.LoadScene("GameOverScene");

            return;
        }

        // =========================
        // 적 사망
        // =========================

        // 추적 AI 정지
        EnemyChaser chaser =
            GetComponent<EnemyChaser>();

        if (chaser != null)
        {
            chaser.enabled = false;
        }

        // 원거리 AI 정지
        EnemyShooter shooter =
            GetComponent<EnemyShooter>();

        if (shooter != null)
        {
            shooter.enabled = false;
        }

        // 적을 삭제하지 않고 시체 상태로 변경
        EnemyCorpse corpse =
            GetComponent<EnemyCorpse>();

        if (corpse == null)
        {
            corpse =
                gameObject.AddComponent<EnemyCorpse>();
        }

        Debug.Log(
            gameObject.name +
            " → 시체가 됨"
        );
    }
}