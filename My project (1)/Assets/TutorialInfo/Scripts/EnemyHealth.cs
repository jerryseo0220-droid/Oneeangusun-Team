using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    private Rigidbody rb;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        // 이미 죽었으면 무시
        if (IsDead)
            return;

        CurrentHealth -= damage;

        Debug.Log(
            gameObject.name +
            " HP : " +
            CurrentHealth
        );

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        CurrentHealth = 0;

        // 적 움직임 정지
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // EnemyCorpse가 없으면 추가
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