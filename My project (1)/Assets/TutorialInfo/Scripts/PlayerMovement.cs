using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    private Vector3 moveDirection;
    private Vector3 lastLookDirection = Vector3.forward;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            lastLookDirection = moveDirection;
        }
    }

    private void FixedUpdate()
    {
        Vector3 nextPosition =
            rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);

        if (lastLookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(lastLookDirection, Vector3.up);

            rb.MoveRotation(targetRotation);
        }
    }

    public Vector3 GetLookDirection()
    {
        return lastLookDirection;
    }
}