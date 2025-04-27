using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 개발자: 이예린
/// 게임의 전체적인 로직을 관리하는 싱글톤 클래스
/// 스테이지 진행 및 게임 흐름을 제어한다
/// </summary>
public class GameManager : NetworkBehaviour, IPlayerJoined
{
    #region Singleton
    static GameManager instance;    // 싱글톤 인스턴스
    public static GameManager Instance => instance;
    #endregion

    #region Variables
    //TODO... Fusion 연결 후 룸 안에 있는 플레이어 받아와 저장
    [SerializeField] List<string> selectableStages; // 선택 가능한 스테이지 목록
    [SerializeField] List<string> selectedStages;   // 선택된 스테이지 목록

    const int MIN_STAGE_COUNT = 3;  // 최소 스테이지 개수
    [SerializeField] int ongoingStage = 0;   // 현재 진행 중인 스테이지 인덱스

    [Header("PlayerSpown")]
    public NetworkObject playerPrefab;
    #endregion

    #region Unity Event
    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 삭제되지 않도록 설정
        }
        else
            Destroy(gameObject);    // 중복 생성 방지
    }
    #endregion

    #region Game Logic
    /// <summary>
    /// 게임을 초기화하고 첫 번째 스테이지를 시작하는 메서드
    /// </summary>
    public void InitializeStagesAndStart()
    {
        SelectRandomUniqueStage();  // 랜덤으로 스테이지 선택
        MoveNextStage();    // 첫 번째 스테이지로 이동
    }

    /// <summary>
    /// 최소 스테이지 개수만큼 랜덤하고 중복되지 않는 스테이지를 선택하는 메서드
    /// </summary>
    private void SelectRandomUniqueStage()
    {
        if (selectableStages.Count == 0)
        {
            Debug.LogWarning("선택할 수 있는 스테이지가 없습니다.");
            return;
        }
        else if (selectableStages.Count < MIN_STAGE_COUNT)
        {
            Debug.LogWarning("최소 스테이지 수를 만족시키지 못했습니다.");
            return;
        }

        for (int i = 0; i < MIN_STAGE_COUNT; i++)
        {
            int stageNum = Random.Range(0, selectableStages.Count);

            // 중복되지 않는 스테이지 선택
            while (selectedStages.Contains(selectableStages[stageNum]))
                stageNum = Random.Range(0, selectableStages.Count);

            selectedStages.Add(selectableStages[stageNum]);
        }
    }

    #region Stage Management
    /// <summary>
    /// 선택된 스테이지 목록에서 다음 스테이지로 이동하는 메서드
    /// </summary>
    private void MoveNextStage()
    {
        Debug.Log("스테이지를 이동합니다.");
        SceneManager.LoadScene(selectedStages[ongoingStage++]);
    }

    /// <summary>
    /// 특정 추가 스테이지로 이동하는 메서드
    /// </summary>
    /// <param name="stageNum">이동할 추가 스테이지의 인덱스</param>
    private void MoveNextStage(int stageNum)
    {
        Debug.Log("추가 스테이지로 이동합니다.");
        SceneManager.LoadScene(selectableStages[stageNum]);
    }
    #endregion

    #region Stage Completion
    /// <summary>
    /// 현재 스테이지가 종료되었을 때 다음 단계를 처리하는 메서드
    /// </summary>
    public void ProcessStageCompletion()
    {
        if (ongoingStage < MIN_STAGE_COUNT)
            MoveNextStage();    // 다음 스테이지 이동
        else
        {
            // TODO... 추가 스테이지를 진행해야 하는지 여부 결정하는 로직 구현
            Debug.Log("게임 종료");
            // TODO... 게임 종료 후 로직 구현
        }
    }
    #endregion

    #endregion

    #region Spawn
    [ContextMenu("Spawn")]
    private void SpawnPlayer(PlayerRef player)
    {
        if (Runner == null)
        {
            Debug.LogError("Runner가 설정되어 있지 않습니다.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab이 설정되어 있지 않습니다.");
            return;
        }

        NetworkObject playerObj = Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);

        if (playerObj == null)
            Debug.Log("플레이어 생성 안 됨");
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner == null)
        {
            Debug.LogError("Runner가 설정되어 있지 않습니다.");
            return;
        }

        // Host만 Spawn 책임을 진다
        if (Runner.IsServer)
        {
            SpawnPlayer(player);
        }
    }
    #endregion
}