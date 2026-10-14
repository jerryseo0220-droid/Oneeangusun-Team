using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    public Slider hpSlider;

    public bool IsDead { get; private set; }

    private Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp;
        UpdateBar();
    }





    public void TakeDamage(int damage)
    {
        // 이미 죽었으면 무시
        if (IsDead)
            return;

        if (currentHp <= 0) return;   // 이미 죽었으면 무시

        currentHp -= damage;
        Debug.Log(name + " HP: " + currentHp);
        UpdateBar();

        if (currentHp <= 0) Die();
    }



    void UpdateBar()
    {
        if (hpSlider != null)
            hpSlider.value = (float)currentHp / maxHp;
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");
            Time.timeScale = 0f;      // 게임 정지
        }
        else
        {
            IsDead = true;
            

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


}
