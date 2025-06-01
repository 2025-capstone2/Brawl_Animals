using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using Fusion.Addons.SimpleKCC;

/// <summary>
/// 개발자: 이예린
/// 
/// 스테이지의 기본 동작을 관리하는 클래스
/// </summary>
public class StageManager : NetworkBehaviour
{
    [Header("StageObju")]
    [SerializeField] private GameObject stageVisualRoot; // 내부 오브젝트 묶음
    public GameObject StageVisualRoot => stageVisualRoot;

    [Header("GameManager")]
    [SerializeField] GameManager gameManager;

    [Header("Time")]
    [Tooltip("스테이지의 전체 타이머 (초 단위로 설정)")]
    [SerializeField] protected float stageTime;

    [Header("Players")]
    [Tooltip("현재 스테이지에서 생존 중인 플레이어들 보관하는 List")]
    [SerializeField] Dictionary<Character, NetworkObject> alivePlayers = new();
    public Dictionary<Character, NetworkObject> AlivePlayers => alivePlayers;

    [Tooltip("플레이어들 스폰 위치 List")]
    [SerializeField] List<Transform> playersSpawnPoints;
    public List<Transform> PlayersSpawnPoints => playersSpawnPoints;

    Coroutine stageLogic = null;    // 스테이지 로직을 실행하는 코루틴 참조
    /// <summary>
    /// 스테이지의 종료 여부를 반환하는 프로퍼티
    /// </summary>
    public NetworkBool IsFinished { get; private set; }

    #region Unity Event
    private void Start()
    {
        IsFinished = true;
    }
    #endregion

    #region Stage Logic
    /// <summary>
    /// 스테이지 로직 실행하는 메서드
    /// </summary>
    public void StartLogic()
    {
        if (gameManager == null)
        {
            Debug.LogError("해당 스테이지에 GameManager가 할당되지 않아 정상적인 게임 로직 실행 불가능");
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
        yield return null;  // 한 프레임 기다리기 (클라이언트의 gameManager.playerCharacters 세팅 기다리기)

        // 스테이지 생존 플레이어 리스트 세팅
        for (int i = 0; i < gameManager.playerCharacters.Count; i++)
        {
            Character character = gameManager.playerCharacters[i].GetComponent<Character>();

            alivePlayers.Add(character, gameManager.playerCharacters[i]);

            character.currentStage = this;
        }

        Debug.Log($"[게임 타이머 시작!!] / {gameObject.name}"); // 타이머 시작 메시지 출력
        float count = 0;    // 타이머 카운트 변수 (초 단위로 진행)
        IsFinished = false;

        // 스테이지 타이머가 끝날 때까지 반복
        while (count < stageTime)
        {
            yield return StartCoroutine(OnStageUpdate(count));

            // 스테이지에 단 한 명의 플레이어만 남았다면, 스테이지 종료
            /*if (alivePlayers.Count == 1)
            {
                Debug.Log("최후의 플레이어 탄생! 게임 스테이지를 종료합니다~");
                stageLogic = null;
                IsFinished = true;
                alivePlayers.Clear();
                yield return StartCoroutine(OnStageEnd());
                gameManager.ProcessStageCompletion();
                yield break;
            }*/
            Debug.Log($"현재 스테이지 종료까지 남은 시간 : {stageTime - count}");   // 남은 시간 출력
            count += 1; // 타이머 카운트 1 증가
            // 타이머 1초씩 증가
            yield return new WaitForSecondsRealtime(1f);
        }

        // 타이머 종료 시 메시지 출력
        Debug.Log("[타임 종료~~]");
        IsFinished = true;
        stageLogic = null;
        alivePlayers.Clear();   // 생존 플레이어 리스트 초기화

        yield return StartCoroutine(OnStageEnd());

        gameManager.ProcessStageCompletion();
    }

    /// <summary>
    /// 각 스테이지의 커스텀 이벤트를 정의하기 위한 확장용 코루틴
    /// 기본 구현은 아무 동작도 하지 않으며, 
    /// 각 스테이지에서 필요한 고유 이벤트(예: 장애물 등장, 기믹 패턴 발동 등)를 정의할 때 override하여 사용
    /// 스테이지 타이머가 진행되는 동안 매 1초마다 호출되며, 경과 시간을 기반으로 조건 분기 처리를 할 수 있음
    /// </summary>
    /// <param name="elapsedTime">스테이지 시작 후 경과된 시간(초)</param>
    /// <returns></returns>
    protected virtual IEnumerator OnStageUpdate(float elapsedTime) { yield break; }

    /// <summary>
    /// 각 스테이지 종료 시 필요한 이벤트 정의햐기 위한 확장용 코루틴
    /// 기본 구현은 아무 동작도 하지 않으며, 
    /// 각 스테이지에서 필요한 고유 종료 이벤트를 정의할 때 override하여 사용
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OnStageEnd() { yield break; }

    /// <summary>
    /// 스테이지 보이는 오브젝트 활성화/비활성화 관리
    /// </summary>
    /// <param name="isActive">활성화 여부</param>
    public void ActivateStage(bool isActive)
    {
        stageVisualRoot.SetActive(isActive); // 시각적으로만 On/Off
        enabled = isActive; // MonoBehaviour 로직도 On/Off
    }
    #endregion

}