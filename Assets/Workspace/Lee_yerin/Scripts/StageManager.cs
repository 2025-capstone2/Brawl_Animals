using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 개발자: 이예린
/// 
/// 스테이지의 기본 동작을 관리하는 클래스
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("스테이지의 전체 타이머 (초 단위로 설정)")]
    [SerializeField] float stageTime;
    [Header("Current players in stage ")]
    [Tooltip("현재 스테이지에서 생존 중인 플레이어들 보관하는 List")]
    [SerializeField] List<Object> alivePlayers = new();

    Coroutine stageLogic = null;    // 스테이지 로직을 실행하는 코루틴 참조
    /// <summary>
    /// 스테이지의 종료 여부를 반환하는 프로퍼티
    /// </summary>
    public bool IsFinished { get; private set; }

    #region Unitye Event
    private void Start()
    {
        // 생존 중인 플레이어가 없거나 리스트가 비어 있으면, 스테이지 실행을 중지하고 메시지 출력
        if (alivePlayers == null || alivePlayers.Count == 0)
        {
            Debug.LogWarning("현재 스테이지 안에 있는 플레이어를 찾을 수 없습니다.");
            return;
        }

        // 스테이지 타임이 0 이하일 경우, 유효하지 않은 값이므로 종료
        if (stageTime <= 0)
        {
            Debug.LogWarning("올바르지 않은 스테이지 플레이 타임이 들어있습니다.");
            return;
        }

        // 스테이지 타이머 및 로직을 실행하는 함수 호출
        StartLogicTimer();
    }
    #endregion

    #region Stage Logic
    /// <summary>
    /// 스테이지 타이머와 로직을 시작하는 메서드
    /// </summary>
    private void StartLogicTimer()
    {
        // 아직 스테이지 로직이 실행되지 않았다면, 코루틴을 시작
        if (stageLogic == null)
            stageLogic = StartCoroutine(StageLogicCoroutine());
        else
            Debug.Log("스테이지가 이미 실행되었습니다.");
    }

    /// <summary>
    /// 스테이지 타이머와 로직을 처리하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator StageLogicCoroutine()
    {
        Debug.Log("[게임 타이머 시작!!]"); // 타이머 시작 메시지 출력
        float count = 0;    // 타이머 카운트 변수 (초 단위로 진행)
        IsFinished = false;

        // 스테이지 타이머가 끝날 때까지 반복
        while (count < stageTime)
        {
            // 스테이지에 단 한 명의 플레이어만 남았다면, 스테이지 종료
            if (alivePlayers.Count == 1)
            {
                Debug.Log("최후의 플레이어 탄생! 게임 스테이지를 종료합니다~");
                stageLogic = null;
                IsFinished = true;
                yield break;
            }
            Debug.Log($"현재 스테이지 종료까지 남은 시간 : {stageTime - count}");   // 남은 시간 출력
            count += 1; // 타이머 카운트 1 증가
            // 타이머 1초씩 증가
            yield return new WaitForSecondsRealtime(1f);
        }

        foreach (var player in alivePlayers)
        {
            Destroy(player);
        }
        // 타이머 종료 시 메시지 출력
        Debug.Log("[타임 종료~~]");
        IsFinished = true;
    }
    #endregion
}