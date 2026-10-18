using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;    // 날아가는 속도
    public int damage = 10;      // 투사체 데미지
    public float lifeTime = 3f;  // 3초 후 자동 소멸 (메모리 누수 방지)

    void Start()
    {
        // 생성된 지 lifeTime 초가 지나면 자동으로 스스로를 파괴
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 앞으로 계속 날아감
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // IsTrigger가 체크된 콜라이더에 닿았을 때 (또는 Rigidbody가 있을 때)
    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 부딪혔다면
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // 플레이어에게 데미지 전달
            }
            Destroy(gameObject); // 맞췄으니 투사체 파괴
        }
        // 만약 바닥이나 돌(Rock) 등에 부딪히면 막히게 하고 싶다면 활성화
        else if (other.CompareTag("Rock") || other.CompareTag("Untagged"))
        {
            // Destroy(gameObject); 
        }
    }
}