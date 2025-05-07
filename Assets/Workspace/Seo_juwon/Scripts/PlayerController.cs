using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Fusion.Addons.SimpleKCC;
/// <summary>
/// 플레이어의 이동 제어 컨트롤러
/// 입력은 외부 InputHandler에서 Fusion을 통해 전달받음
/// kccSample 참고함
/// </summary>
public class PlayerController : NetworkBehaviour
{
    public SimpleKCC kcc; // SimpleKCC 컴포넌트
    public Character character; //캐릭터 구분

    [Header("Movement Settings")]
    public float moveSpeed = 10f; // 이동 속도
    public float jumpImpulse = 10f; //혹시 모를 점프 변수
    public float upGravity = -25f; //kcc 중력 관련 변수
    public float downGravity = -40f; //kcc 중력 관련 변수
    public float groundAcceleration = 50f; //kcc 관련 땅에 있을 때 가속도
    public float groundDeceleration = 25f; //kcc 관련 땅에 있을 때 가속도
    public float airAcceleration = 20f; //kcc 관련 떠 있을 때 가속도
    public float airDeceleration = 2f; //kcc 관련 떠 있을 때 가속도

    [Networked]
    private Vector3 moveVelocity { get; set; }

    public override void FixedUpdateNetwork()
    {
        //GetInput 받아오지 못했을 때
        if (!GetInput<NetworkInputData>(out var inputData))
        {
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
        if (inputData.skill)
        {
            character.UseSkill();  // 각 캐릭터마다 고유 스킬 사용
        }
        if (GetInput(out NetworkInputData data))
        {
            if (data.attack)
            {
                character.TryAttack(); //공격
            }
        }
    }
}
