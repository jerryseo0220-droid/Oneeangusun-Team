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
    [SerializeField] private PlayerController player;
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
    private readonly List<EnemyCorpse> attachedCorpses = new();

    private readonly HashSet<Health> hitEnemiesThisThrow = new();

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

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

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowPlanet();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RecallPlanet();
        }
    }

    private void FollowPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.PlantTarget.forward;
        Vector3 targetPosition = player.transform.position + direction * followDistance;

        targetPosition.y = rb.position.y;

        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            targetPosition,
            followSpeed * Time.fixedDeltaTime
        );

        nextPosition.y = rb.position.y + (rb.linearVelocity.y * Time.fixedDeltaTime);

        rb.MovePosition(nextPosition);
    }

    private void ThrowPlanet()
    {
        if (currentState != PlanetState.Rolling)
            return;

        throwDirection = player.PlantTarget.forward;
        throwDirection.y = 0f;
        throwDirection.Normalize();

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

    private void RecallPlanet()
    {
        if (currentState != PlanetState.Thrown)
            return;

        Vector3 direction = player.PlantTarget.forward;

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

    private void RollPlanet()
    {
        Vector3 movement = rb.position - lastPosition;

        if (movement.sqrMagnitude > 0.0001f)
        {
            Vector3 rollAxis = Vector3.Cross(Vector3.up, movement.normalized);
            transform.Rotate(rollAxis, rollSpeed * movement.magnitude, Space.World);
        }

        lastPosition = rb.position;
    }

    // 3의 제곱으로 레벨업 요구치 계산
    private int GetItemsNeededToGrow()
    {
        return (int)Mathf.Pow(3, planetLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject, other.GetComponentInParent<Health>(), other.GetComponentInParent<EnemyCorpse>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject, collision.collider.GetComponentInParent<Health>(), collision.collider.GetComponentInParent<EnemyCorpse>());
    }

    private void HandleCollision(GameObject obj, Health enemy, EnemyCorpse corpse)
    {
        if (currentState == PlanetState.Thrown)
        {
            if (enemy != null && !enemy.IsDead)
            {
                if (hitEnemiesThisThrow.Add(enemy))
                {
                    enemy.TakeDamage(attackDamage);
                    Debug.Log("적 공격! 데미지 : " + attackDamage);
                }
            }
            return;
        }

        if (currentState != PlanetState.Rolling)
            return;

        if (corpse != null && !corpse.IsCollected)
        {
            AttachCorpse(corpse);
            return;
        }

        if (obj.CompareTag("Rock"))
        {
            AttachRock(obj);
        }
    }

    private void AttachRock(GameObject rock)
    {
        rock.tag = "Untagged";

        Rigidbody rockRb = rock.GetComponent<Rigidbody>();
        if (rockRb != null)
        {
            rockRb.linearVelocity = Vector3.zero;
            rockRb.angularVelocity = Vector3.zero;
            rockRb.useGravity = false;
            rockRb.isKinematic = true;
        }

        Collider rockCollider = rock.GetComponent<Collider>();
        if (rockCollider != null)
        {
            rockCollider.enabled = false;
        }

        rock.transform.SetParent(transform, true);
        attachedRocks.Add(rock);
        attachedRockCount++;

        // [추가됨] 돌을 먹을 때마다 즉시 10점 획득 (돌 1개, 시체 0개 전달)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1, 0);
        }

        int totalItems = attachedRockCount + attachedCorpseCount;
        int itemsNeeded = GetItemsNeededToGrow();
        Debug.Log($"오브젝트 흡수 (돌) -> 현재: {totalItems}개 / 필요: {itemsNeeded}개");

        if (totalItems >= itemsNeeded)
        {
            GrowPlanet();
        }
    }

    private void AttachCorpse(EnemyCorpse corpse)
    {
        corpse.Collect();
        GameObject corpseObject = corpse.gameObject;

        Rigidbody corpseRb = corpseObject.GetComponent<Rigidbody>();
        if (corpseRb != null)
        {
            corpseRb.linearVelocity = Vector3.zero;
            corpseRb.angularVelocity = Vector3.zero;
            corpseRb.useGravity = false;
            corpseRb.isKinematic = true;
        }

        Collider[] colliders = corpseObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        corpseObject.transform.SetParent(transform, true);
        attachedCorpses.Add(corpse);
        attachedCorpseCount++;

        // [추가됨] 시체를 먹을 때마다 즉시 15점 획득 (돌 0개, 시체 1개 전달)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(0, 1);
        }

        int totalItems = attachedRockCount + attachedCorpseCount;
        int itemsNeeded = GetItemsNeededToGrow();
        Debug.Log($"오브젝트 흡수 (시체) -> 현재: {totalItems}개 / 필요: {itemsNeeded}개");

        if (totalItems >= itemsNeeded)
        {
            GrowPlanet();
        }
    }

    private void GrowPlanet()
    {
        planetLevel++;

        transform.localScale += Vector3.one * growAmount;

        transform.position += new Vector3(0, growAmount / 2f, 0);

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
            col.enabled = true;
        }

        // [삭제됨] 레벨업 시에 한 번에 점수를 주던 코드는 제거했습니다.

        foreach (GameObject rock in attachedRocks)
        {
            if (rock != null) Destroy(rock);
        }
        attachedRocks.Clear();
        attachedRockCount = 0;

        foreach (EnemyCorpse corpse in attachedCorpses)
        {
            if (corpse != null) Destroy(corpse.gameObject);
        }
        attachedCorpses.Clear();
        attachedCorpseCount = 0;

        Debug.Log($"행성 폭발적 성장! [레벨 {planetLevel}] 다음 레벨업 필요 오브젝트 수: {GetItemsNeededToGrow()}개");
    }
}