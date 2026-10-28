using UnityEngine;
using UnityEngine.UI;

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
            hpSlider.value = (float)currentHp / maxHp;
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
            Debug.Log("플레이어 사망! 게임 매니저를 호출합니다.");

            // [수정된 부분] 씬을 직접 넘기지 않고 GameManager에게 점수 저장과 씬 전환을 통째로 맡깁니다!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGameOver();
            }

            return;
        }

        // =========================
        // 적 사망
        // =========================

        // 추적 AI 정지
        EnemyChaser chaser = GetComponent<EnemyChaser>();
        if (chaser != null)
        {
            chaser.enabled = false;
        }

        // 원거리 AI 정지
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
        {
            shooter.enabled = false;
        }

        // 적을 삭제하지 않고 시체 상태로 변경
        EnemyCorpse corpse = GetComponent<EnemyCorpse>();
        if (corpse == null)
        {
            corpse = gameObject.AddComponent<EnemyCorpse>();
        }

        Debug.Log(gameObject.name + " → 시체가 됨");
    }
}