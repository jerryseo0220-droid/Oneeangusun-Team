using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("이동 및 인식 설정")]
    public float moveSpeed = 2f;        // 근거리 적보다 보통 약간 느리게 설정
    public float detectRange = 20f;     // 플레이어 인식 거리
    public float attackRange = 10f;     // 공격 사거리 (이 거리 안이면 총을 쏨)

    [Header("공격 설정")]
    public float attackCooldown = 2f;   // 발사 쿨타임
    public GameObject projectilePrefab; // 발사할 투사체(총알) 프리팹
    public Transform firePoint;         // 총알이 발사될 위치

    private Transform player;
    private float lastAttackTime = -999f;
    private Health myHealth;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        myHealth = GetComponent<Health>();
    }

    void Update()
    {
        // 플레이어가 없거나, 나(적) 자신이 죽은 상태면 작동안함
        if (player == null || (myHealth != null && myHealth.IsDead)) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0; // 높이 무시
        float distance = toPlayer.magnitude;

        // 인식 거리 밖이면 대기
        if (distance > detectRange) return;

        // 항상 플레이어를 바라봄
        transform.forward = toPlayer.normalized;

        // 사거리보다 멀면 다가감
        if (distance > attackRange)
        {
            float move = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * move;
        }
        // 사거리 안이고, 쿨타임이 찼다면 공격(발사)
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        lastAttackTime = Time.time;

        if (projectilePrefab != null && firePoint != null)
        {
            // 발사 위치(firePoint)에서, 발사 위치의 방향(rotation)을 바라보는 투사체 생성
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log(gameObject.name + "가 원거리 공격을 발사했습니다!");
        }
    }
}