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
public class GameManager : NetworkBehaviour
{
    #region Variables
    //TODO... Fusion 연결 후 룸 안에 있는 플레이어 받아와 저장
    [SerializeField] List<StageManager> selectableStages; // 선택 가능한 스테이지 목록
    [SerializeField] List<StageManager> selectedStages;   // 선택된 스테이지 목록

    const int MIN_STAGE_COUNT = 3;  // 최소 스테이지 개수
    [SerializeField] int ongoingStage = -1;   // 현재 진행 중인 스테이지 인덱스
    #endregion

    #region Unity Event
    #endregion

    #region Game Logic
    /// <summary>
    /// 게임을 초기화하고 첫 번째 스테이지를 시작하는 메서드
    /// </summary>
    [ContextMenu("InitializeStagesAndStart")]
    public void InitializeStagesAndStart()
    {
        Debug.Log("InitializeStagesAndStart");

        if (Runner == null)
            Debug.Log("Runner is Null");
        else
            Debug.Log(Runner.gameObject.name);

        if (Runner.IsServer && HasStateAuthority)
        {
            Debug.Log("HasStateAuthority");
            SelectRandomUniqueStage();  // 랜덤으로 스테이지 선택
        }
        //MoveNextStage();            // 첫 번째 스테이지로 이동
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

        int[] chosenStages = new int[MIN_STAGE_COUNT];

        for (int i = 0; i < MIN_STAGE_COUNT; i++)
        {
            int stageNum = Random.Range(0, selectableStages.Count);

            // 중복되지 않는 스테이지 선택
            while (selectedStages.Contains(selectableStages[stageNum]))
                stageNum = Random.Range(0, selectableStages.Count);

            chosenStages[i] = stageNum;
        }

        RPC_BroadcastSelectedStages(chosenStages);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_BroadcastSelectedStages(int [] chosenStages)
    {
        selectedStages.Clear();
        foreach (int idx in chosenStages)
            selectedStages.Add(selectableStages[idx]);
    }

    #region Stage Management
    /// <summary>
    /// 선택된 스테이지 목록에서 다음 스테이지로 이동하는 메서드
    /// </summary>
    private void MoveNextStage()
    {
        Debug.Log("스테이지를 이동합니다.");

        //TODO... 다음 스테이지 로딩 UI 활성화
        if (ongoingStage >= 0)
            selectedStages[ongoingStage].gameObject.SetActive(false);   // 현재 스테이지 비활성화

        selectedStages[++ongoingStage].gameObject.SetActive(true);   // 다음 스테이지 활성화
        //TODO... 다음 스테이지 로딩 UI 비활성화
    }

    /// <summary>
    /// 특정 추가 스테이지로 이동하는 메서드
    /// </summary>
    /// <param name="stageNum">이동할 추가 스테이지의 인덱스</param>
    private void MoveNextStage(int stageNum)
    {
        Debug.Log("추가 스테이지로 이동합니다.");
        
        //TODO... 다음 스테이지 로딩 UI 활성화
        selectedStages.Add(selectableStages[stageNum]); // 추가로 선택된 스테이지를 selectedStages 리스트에 추가
        selectedStages[ongoingStage].gameObject.SetActive(false);   // 현재 스테이지 비활성화
        selectedStages[++ongoingStage].gameObject.SetActive(true);   // 다음 스테이지 활성화
        //TODO... 다음 스테이지 로딩 UI 활성화

    }
    #endregion

    #region Stage Completion
    /// <summary>
    /// 현재 스테이지가 종료되었을 때 다음 단계를 처리하는 메서드
    /// </summary>
    public void ProcessStageCompletion()
    {
        if (ongoingStage + 1 < MIN_STAGE_COUNT)
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
}