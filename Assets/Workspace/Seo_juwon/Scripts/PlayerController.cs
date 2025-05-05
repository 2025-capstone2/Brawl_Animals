using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Fusion.Addons.SimpleKCC;

public class PlayerController : NetworkBehaviour
{
    public SimpleKCC kcc;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpImpulse = 10f;
    public float upGravity = -25f;
    public float downGravity = -40f;
    public float groundAcceleration = 50f;
    public float groundDeceleration = 25f;
    public float airAcceleration = 20f;
    public float airDeceleration = 2f;

    [Networked]
    private Vector3 moveVelocity { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput<NetworkInputData>(out var inputData))
        {
            Debug.LogWarning("GetInput 실패");
            return;
        }
        // 방향 입력
        Vector3 inputDirection = kcc.TransformRotation * new Vector3(inputData.moveInput.x, 0, inputData.moveInput.y);
        inputDirection.Normalize();

        // 중력 적용
        kcc.SetGravity(kcc.RealVelocity.y >= 0f ? upGravity : downGravity);

        // 이동 속도 계산
        Vector3 desiredVelocity = inputDirection * moveSpeed;

        if (kcc.ProjectOnGround(desiredVelocity, out Vector3 projected))
        {
            desiredVelocity = projected.normalized * moveSpeed;
        }

        // 가속도 계산
        float accel = desiredVelocity == Vector3.zero
            ? (kcc.IsGrounded ? groundDeceleration : airDeceleration)
            : (kcc.IsGrounded ? groundAcceleration : airAcceleration);

        moveVelocity = Vector3.Lerp(moveVelocity, desiredVelocity, accel * Runner.DeltaTime);

        // 실제 이동
        kcc.Move(moveVelocity);
    }
}
