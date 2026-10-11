using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Rigidbody rb;

    [Header("Follow")]
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float followSpeed = 12f;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 300f;

    [Header("Growth")]
    [SerializeField] private int objectsNeededToGrow = 5;
    [SerializeField] private float growAmount = 0.4f;

    private int attachedObjectCount;
    private int planetLevel = 1;

    private Vector3 lastPosition;

    private readonly List<GameObject> attachedObjects = new();

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        lastPosition = rb.position;
    }

    private void FixedUpdate()
    {
        FollowPlayer();
        RollPlanet();
    }

    private void FollowPlayer()
    {
        Vector3 lookDirection = player.GetLookDirection();

        Vector3 targetPosition =
            player.transform.position +
            lookDirection * followDistance;

        targetPosition.y = rb.position.y;

        Vector3 newPosition = Vector3.Lerp(
            rb.position,
            targetPosition,
            followSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    private void RollPlanet()
    {
        Vector3 movement = rb.position - lastPosition;

        if (movement.sqrMagnitude > 0.0001f)
        {
            Vector3 rollAxis =
                Vector3.Cross(Vector3.up, movement.normalized);

            transform.Rotate(
                rollAxis,
                rollSpeed * movement.magnitude,
                Space.World
            );
        }

        lastPosition = rb.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Rock"))
            return;

        AttachObject(other.gameObject);
    }

    private void AttachObject(GameObject obj)
    {
        // 같은 돌이 여러 번 처리되지 않도록
        obj.tag = "Untagged";

        // 기존 물리 제거
        Collider col = obj.GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        Rigidbody objectRb = obj.GetComponent<Rigidbody>();

        if (objectRb != null)
        {
            objectRb.isKinematic = true;
            objectRb.useGravity = false;
        }

        // 현재 닿아있는 위치 그대로 행성에 부착
        obj.transform.SetParent(transform, true);

        attachedObjects.Add(obj);

        attachedObjectCount++;

        Debug.Log(
            $"붙은 물체: {attachedObjectCount} / {objectsNeededToGrow}"
        );

        if (attachedObjectCount >= objectsNeededToGrow)
        {
            GrowPlanet();
        }
    }

    private void GrowPlanet()
    {
        planetLevel++;

        // 행성 본체 크기 증가
        transform.localScale += Vector3.one * growAmount;

        // 지금까지 표면에 붙어있던 물체를 본체에 합쳐졌다고 처리
        foreach (GameObject obj in attachedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        attachedObjects.Clear();

        attachedObjectCount = 0;

        Debug.Log($"행성 {planetLevel}단계 성장");
    }
}