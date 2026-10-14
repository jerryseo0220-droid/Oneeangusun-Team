using UnityEngine;

public class EnemyChaser : MonoBehaviour
{

    public float moveSpeed = 3f;
    public float detectRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int damage = 1;

    Transform player;
    float lastAttackTime = -999f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

    }

    // Update is called once per frame
    void Update()
    {

        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;                     // 높이 무시
        float distance = toPlayer.magnitude;

        if (distance > detectRange) return; // 대기

        transform.forward = toPlayer.normalized;

        if (distance > attackRange)         // 추적
        {
            float move = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * move;
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {                                   // 공격
            lastAttackTime = Time.time;
            player.GetComponent<Health>().TakeDamage(damage);
        }
    }

}

