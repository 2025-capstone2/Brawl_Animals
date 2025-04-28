using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementInputAction : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInput playerInput; // 필드로 선언

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>(); // 여기서 필드에 저장
    }

    private void Update()
    {
        // 입력은 Update()에서만 읽기
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Debug.Log($"Move Input: {moveInput}");
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);
        rb.velocity = velocity;
    }
}