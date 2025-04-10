using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terresquall;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }
    private void Update()
    {
        // VirtualJoystick에서 입력 받아오기
        float horizontal = VirtualJoystick.GetAxis("Horizontal");
        float vertical = VirtualJoystick.GetAxis("Vertical");

        // 방향 벡터 만들기
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // 실제 이동 처리
        if (direction.magnitude > 0.1f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
        // 스킬 사용 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.UseSkill();
        }
    }
}