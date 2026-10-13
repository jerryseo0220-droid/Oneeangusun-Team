using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    private enum PlanetState
    {
        Rolling,
        Thrown,
        Returning
    }

    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Rigidbody rb;

    [Header("Follow")]
    [SerializeField] private float followDistance = 2.5f;
    [SerializeField] private float followSpeed = 20f;

    [Header("Throw")]
    [SerializeField] private float throwSpeed = 12f;

    [Header("Return")]
    [SerializeField] private float returnSpeed = 15f;
    [SerializeField] private float returnCompleteDistance = 0.15f;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 300f;

    [Header("Growth")]
    [SerializeField] private int baseRocksNeeded = 3;
    [SerializeField] private int rocksIncreasePerLevel = 2;
    [SerializeField] private float growAmount = 0.4f;

    [Header("Attack")]
    [SerializeField] private int attackDamage = 10;

    private PlanetState currentState = PlanetState.Rolling;

    private Vector3 throwDirection;
    private Vector3 returnTarget;
    private Vector3 lastPosition;

    private int attachedRockCount = 0;
    private int attachedCorpseCount = 0;
    private int planetLevel = 1;

    private readonly List<GameObject> attachedRocks = new();

    // 한 번 던졌을 때 같은 적 중복 공격 방지
    private readonly HashSet<EnemyHealth> hitEnemiesThisThrow = new();

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        lastPosition = rb.position;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlanetState.Rolling:
                FollowPlayer();
                break;

            case PlanetState.Thrown:
                MoveThrownPlanet();
                break;

            case PlanetState.Returning:
                MoveReturningPlanet();
                break;
        }

        RollPlanet();
    }

    // =========================
    // 입력
    // =========================
    private void HandleInput()
    {
        // 좌클릭 = 던지기
        if (Input.GetMouseButtonDown(0))
        {
            ThrowPlanet();
        }

        // 우클릭 = 회수
        if (Input.GetMouseButtonDown(1))
        {
            RecallPlanet();
        }
    }

    // =========================
    // 플레이어 앞 따라가기
    // =========================
    private void FollowPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.GetLookDirection();

        Vector3 targetPosition =
            player.transform.position +
            direction * followDistance;

        targetPosition.y = rb.position.y;

        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            targetPosition,
            followSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    // =========================
    // 좌클릭 - 직선 투척
    // =========================
    private void ThrowPlanet()
    {
        if (currentState != PlanetState.Rolling)
            return;

        throwDirection = player.GetLookDirection();

        throwDirection.y = 0f;
        throwDirection.Normalize();

        // 새로운 투척이므로 맞힌 적 목록 초기화
        hitEnemiesThisThrow.Clear();

        currentState = PlanetState.Thrown;

        Debug.Log("행성 투척");
    }

    private void MoveThrownPlanet()
    {
        Vector3 nextPosition =
            rb.position +
            throwDirection *
            throwSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    // =========================
    // 우클릭 - 직선 회수
    // =========================
    private void RecallPlanet()
    {
        if (currentState != PlanetState.Thrown)
            return;

        Vector3 direction = player.GetLookDirection();

        returnTarget =
            player.transform.position +
            direction * followDistance;

        returnTarget.y = rb.position.y;

        currentState = PlanetState.Returning;

        Debug.Log("행성 회수");
    }

    private void MoveReturningPlanet()
    {
        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            returnTarget,
            returnSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        float distance =
            Vector3.Distance(
                rb.position,
                returnTarget
            );

        if (distance <= returnCompleteDistance)
        {
            rb.MovePosition(returnTarget);

            currentState = PlanetState.Rolling;

            Debug.Log("행성 귀환 완료");
        }
    }

    // =========================
    // 행성 굴러가기
    // =========================
    private void RollPlanet()
    {
        Vector3 movement =
            rb.position - lastPosition;

        if (movement.sqrMagnitude > 0.0001f)
        {
            Vector3 rollAxis =
                Vector3.Cross(
                    Vector3.up,
                    movement.normalized
                );

            transform.Rotate(
                rollAxis,
                rollSpeed * movement.magnitude,
                Space.World
            );
        }

        lastPosition = rb.position;
    }

    // =========================
    // 현재 단계 성장 필요 돌 개수
    // =========================
    private int GetRocksNeededToGrow()
    {
        return baseRocksNeeded +
               (planetLevel - 1) * rocksIncreasePerLevel;
    }

    // =========================
    // 충돌
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        // =========================
        // 던진 상태 → 적 공격
        // =========================
        if (currentState == PlanetState.Thrown)
        {
            if (enemy != null && !enemy.IsDead)
            {
                if (hitEnemiesThisThrow.Add(enemy))
                {
                    enemy.TakeDamage(attackDamage);

                    Debug.Log(
                        "적 공격! 데미지 : " +
                        attackDamage
                    );
                }

                return;
            }
        }

        // 굴리는 상태에서만 재료 수집
        if (currentState != PlanetState.Rolling)
            return;

        // =========================
        // 죽은 적 시체 획득
        // =========================
        EnemyCorpse corpse =
            other.GetComponentInParent<EnemyCorpse>();

        if (corpse != null && !corpse.IsCollected)
        {
            AttachCorpse(corpse);
            return;
        }

        // =========================
        // 돌 획득
        // =========================
        if (other.CompareTag("Rock"))
        {
            AttachRock(other.gameObject);
        }
    }

    // =========================
    // 돌 붙이기
    // =========================
    private void AttachRock(GameObject rock)
    {
        rock.tag = "Untagged";

        Rigidbody rockRb =
            rock.GetComponent<Rigidbody>();

        if (rockRb != null)
        {
            rockRb.linearVelocity = Vector3.zero;
            rockRb.angularVelocity = Vector3.zero;
            rockRb.useGravity = false;
            rockRb.isKinematic = true;
        }

        Collider rockCollider =
            rock.GetComponent<Collider>();

        if (rockCollider != null)
        {
            rockCollider.enabled = false;
        }

        // 닿은 위치 그대로 붙이기
        rock.transform.SetParent(transform, true);

        attachedRocks.Add(rock);
        attachedRockCount++;

        int rocksNeeded = GetRocksNeededToGrow();

        Debug.Log(
            "붙은 돌 : " +
            attachedRockCount +
            " / " +
            rocksNeeded
        );

        if (attachedRockCount >= rocksNeeded)
        {
            GrowPlanet();
        }
    }

    // =========================
    // 시체 붙이기
    // =========================
    private void AttachCorpse(EnemyCorpse corpse)
    {
        corpse.Collect();

        GameObject corpseObject =
            corpse.gameObject;

        Rigidbody corpseRb =
            corpseObject.GetComponent<Rigidbody>();

        if (corpseRb != null)
        {
            corpseRb.linearVelocity = Vector3.zero;
            corpseRb.angularVelocity = Vector3.zero;
            corpseRb.useGravity = false;
            corpseRb.isKinematic = true;
        }

        Collider[] colliders =
            corpseObject.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // 시체도 닿은 위치 그대로 행성에 붙음
        corpseObject.transform.SetParent(
            transform,
            true
        );

        attachedCorpseCount++;

        Debug.Log(
            "시체 획득 : " +
            attachedCorpseCount
        );
    }

    // =========================
    // 행성 성장
    // =========================
    private void GrowPlanet()
    {
        planetLevel++;

        transform.localScale +=
            Vector3.one * growAmount;

        // 성장에 사용한 돌 제거
        foreach (GameObject rock in attachedRocks)
        {
            if (rock != null)
            {
                Destroy(rock);
            }
        }

        attachedRocks.Clear();
        attachedRockCount = 0;

        Debug.Log(
            "행성 성장! 현재 단계 : " +
            planetLevel +
            " / 다음 성장 필요 돌 : " +
            GetRocksNeededToGrow()
        );
    }
}