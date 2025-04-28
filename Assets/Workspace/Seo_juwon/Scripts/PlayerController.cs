using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    public void OnMove(InputValue value)
    {
        // InputValue을 통해 입력받기
        moveInput = value.Get<Vector2>();
    }

    private void MoveCharacter()
    {
        // 물리적인 이동 처리
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);
        rb.velocity = velocity;
    }
}