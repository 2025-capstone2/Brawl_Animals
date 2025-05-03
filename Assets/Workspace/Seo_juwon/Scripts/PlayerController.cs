using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 10f;  // 이동 속도

    private Vector2 moveInput;

    public void Start()
    {
        if (HasInputAuthority)  // 자신이 이 플레이어의 소유자인 경우에만 처리
        {
            Debug.Log("조작 가능한 내 것");
        }
        else Debug.Log("조작 불가능");

    }

    // 네트워크에서 물리적 이동을 처리
    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)  // 자신이 이 플레이어의 소유자인 경우에만 처리
        {
            HandleMovement();
        }
    }
    // 이동 처리 함수
    private void HandleMovement()
    {
        // 입력을 받아서 이동 방향 결정
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection.Normalize();  // 방향만 정규화
        Debug.Log("Move Input: " + moveDirection);
        // 물리적으로 이동 처리
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // PlayerInput으로부터 이동 입력을 받는 함수
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}