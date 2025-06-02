using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개발자: 이예린
/// 게임의 전체적인 로직을 관리하는 싱글톤 클래스
/// 스테이지 진행 및 게임 흐름을 제어한다
/// </summary>
public class GameManager : NetworkBehaviour
{
    #region Variables
    [Header("Game Logic")]
    //TODO... Fusion 연결 후 룸 안에 있는 플레이어 받아와 저장
    [SerializeField] List<StageManager> selectableStages; // 선택 가능한 스테이지 목록
    [SerializeField] List<StageManager> selectedStages;   // 선택된 스테이지 목록

    const int MIN_STAGE_COUNT = 3;  // 최소 스테이지 개수
    [SerializeField] public int ongoingStage = -1;   // 현재 진행 중인 스테이지 인덱스

    [SerializeField] public List<NetworkObject> playerCharacters;
    [SerializeField] PlayerSpawner playerSpawner;
    private Dictionary<PlayerRef, Animal> playerPickList = new();
    public Dictionary<PlayerRef, Animal> PlayerPickList => playerPickList;

    [Header("UI")]
    [SerializeField] GameObject finishUI;
    #endregion

    #region Unity Event
    #endregion

    #region Chracter Pick
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPCAddPlayerPick(PlayerRef player, int animal)
    {
        PlayerPickList.Add(player, (Animal) animal);
    }
    #endregion

    #region Game Logic
    public void InitializeStagesAndStart()
    {
        RPC_InitializeStagesAndStart();
    }

    /// <summary>
    /// 게임을 초기화하고 첫 번째 스테이지를 시작하는 메서드
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_InitializeStagesAndStart()
    {
        playerSpawner = Runner.gameObject.GetComponent<PlayerSpawner>();
        playerSpawner.playerPickList = playerPickList;

        // 서버이자 해당 NetworkObject의 StateAuthority를 가진 경우에만 실행
        if (!(Runner.IsServer && HasStateAuthority))
            return;

        Debug.Log("InitializeStagesAndStart");

        SelectRandomUniqueStage();  // 랜덤으로 스테이지 선택

        RpcMoveNextStage(); // 첫 번째 스테이지로 이동
        Debug.Log(playerCharacters.Count);
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

        List<int> chosenStages = new ();

        for (int i = 0; i < MIN_STAGE_COUNT; i++)
        {
            int stageNum = Random.Range(0, selectableStages.Count);

            // 중복되지 않는 스테이지 선택
            while (chosenStages.Contains(stageNum))
                stageNum = Random.Range(0, selectableStages.Count);

            chosenStages.Add(stageNum);
        }

        RPC_BroadcastSelectedStages(chosenStages.ToArray());
    }

    /// <summary>
    /// 서버에서 선택한 스테이지 인덱스 배열을 모든 클라이언트에 동기화하는 RPC 메서드
    /// 선택된 인덱스를 기반으로 selectedStages 리스트를 클라이언트마다 동일하게 구성
    /// </summary>
    /// <param name="chosenStages">서버가 선정한 인덱스 배열</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_BroadcastSelectedStages(int [] chosenStages)
    {
        selectedStages.Clear();
        foreach (int idx in chosenStages)
            selectedStages.Add(selectableStages[idx]);
    }

    #region Stage Management
    /// <summary>
    /// 선택된 스테이지 목록에서 다음 스테이지로 이동하는 RPC 메서드
    /// 서버(StateAuthority)에서 호출되며, 모든 클라이언트에게 스테이지 전환을 동기화함.
    /// isExtraStage가 true일 경우, 추가 스테이지를 동적으로 선택하여 목록에 포함시킴.
    /// </summary>
    /// <param name="isExtraStage">true이면 선택 가능한 스테이지 중 하나를 추가로 선택함</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcMoveNextStage(bool isExtraStage = false)
    {
        // 추가 스테이지가 필요한 경우, 랜덤으로 하나를 선택해 selectedStages에 추가
        if (isExtraStage)
        {
            int extraStageNum = Random.Range(0, selectableStages.Count);
            selectedStages.Add(selectableStages[extraStageNum]); // 추가로 선택된 스테이지를 selectedStages 리스트에 추가
        }

        // 이전 스테이지가 존재하면 비활성화
        if (ongoingStage >= 0 && ongoingStage < selectedStages.Count)
        {
            selectedStages[ongoingStage].ActivateStage(false);

            foreach (NetworkObject player in playerCharacters)
            {
                Runner.Despawn(player);
            }
        }
        else
            SoundManager.Instance.PlayBGM(bgmClip.game);

        // 다음 스테이지로 진행
        ongoingStage++;

        // 다음 스테이지가 존재하면 활성화
        if (ongoingStage < selectedStages.Count)
            selectedStages[ongoingStage].ActivateStage(true);

        playerCharacters.Clear();

        if (Runner.IsServer)
        {
            foreach (NetworkObject character in playerSpawner?.SpawnAllPendingPlayers(selectedStages[ongoingStage].PlayersSpawnPoints))
            {
                playerCharacters.Add(character);
            }
        }
        else
        {
            // 클라이언트 playerCharacters 동기화 작업 실행
            StartCoroutine(RebuildPlayerCharactersNextFrame());
        }

            selectedStages[ongoingStage].StartLogic();  // 스테이지 로직 실행
    }

    /// <summary>
    /// 클라이언트에서 Fusion 동기화가 완료된 이후,
    /// 각 플레이어에 대응하는 NetworkObject를 수집하여 playerCharacters 리스트를 재구성하는 코루틴입니다.
    /// Fusion의 SetPlayerObject 등록 이후 동기화 지연을 고려하여 한 프레임 뒤에 실행됩니다.
    /// </summary>
    /// <returns>코루틴 대기를 위한 IEnumerator</returns>
    private IEnumerator RebuildPlayerCharactersNextFrame()
    {
        yield return null;

        playerCharacters.Clear();

        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out var obj))
            {
                playerCharacters.Add(obj);
            }
            else
            {
                Debug.LogWarning($"[Client] Player {player.PlayerId}의 오브젝트를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"[Client] playerCharacters 복구 완료: {playerCharacters.Count}개");
    }
    #endregion

    #region Stage Completion
    /// <summary>
    /// 현재 스테이지가 종료되었을 때 다음 단계를 처리하는 메서드
    /// </summary>
    public void ProcessStageCompletion()
    {
        if (!(Runner.IsServer && HasStateAuthority))
            return;

        if (ongoingStage + 1 < MIN_STAGE_COUNT)
            RpcMoveNextStage();    // 다음 스테이지 이동
        else
        {
            // TODO... 추가 스테이지를 진행해야 하는지 여부 결정하는 로직 구현
            Debug.Log("게임 종료");
            RpcFinishLogic();  // 게임 종료 로직
            // TODO... 게임 종료 후 로직 구현
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcFinishLogic()
    {
        finishUI.SetActive(true);
        SoundManager.Instance.PlayBGM(bgmClip.gameEnd);
    }
    #endregion

    #endregion
}