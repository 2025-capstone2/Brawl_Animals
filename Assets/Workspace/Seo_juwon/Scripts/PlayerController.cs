using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

/// <summary>
/// 각 플레이어가 자신의 입력을 통해 Rigidbody로 움직이는 네트워크 플레이어 컨트롤러
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]
public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
            return; // 내가 조작할 권한이 없으면 움직이지 않는다

        MoveCharacter();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
     public void OnInput(NetworkRunner runner, NetworkInput input)
    {
         var data = new PlayerInputData
        {
            Move = moveInput
        };
        input.Set(data);
    }
    private void MoveCharacter()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);
        rb.velocity = velocity;
    }

}