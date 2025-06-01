using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 개발자: 이예린
/// 
/// 무덤 맵의 StageManager로,
/// 고스트 AI 생성 및 플레이어 추적 로직 및 스테이지 이벤트 로직 관리함
/// </summary>
public class GraveyardStageManager : StageManager
{
    [Header("Ghost AI NPC")]
    [SerializeField] GameObject ghostAIPrefab;  // 고스트 프리팹
    [SerializeField] List<GhostAI> ghostAI;     // 멥 안에 있는 고스트 리스트 
    [SerializeField] Transform ghostSpawnPoint;     // 고스트 스폰 위치

    [SerializeField] private List<NetworkObject> closestCandidates = new();
    [SerializeField] private List<NetworkObject> currentTarget = new();

    #region Unity Event
    private void FixedUpdate()
    {
        if (IsFinished) // 스테이지 실행 중이 아닐 경우
            return;
        if (!HasStateAuthority) // Host가 아닐 경우
            return;

        UpdateGhostTarget();    // Ghost 타켓 탐색
        MoveGhostToTarget();    // Ghost를 타켓으로 이동
    }
    #endregion

    #region Logic Event
    protected override IEnumerator OnStageUpdate(float elapsedTime)
    {
        if (!HasStateAuthority) // Host가 아닐 경우
            yield break;

        if (elapsedTime == 0 || elapsedTime == (int) (stageTime / 2))   // 스테이지 시작 또는 스테이지 타임이 절반 지났을 때
        {
            NetworkObject ghostObj = Runner.Spawn(ghostAIPrefab, ghostSpawnPoint.position, Quaternion.identity);    // 고스트 Spawn
            closestCandidates.Add(null);
            currentTarget.Add(null);
            ghostAI.Add(ghostObj.GetComponent<GhostAI>());

            RpcSyncGhostTrackingState(ghostObj.Id);
        }
        else
            yield break;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcSyncGhostTrackingState(NetworkId ghostId)
    {
        if (HasStateAuthority)
            return;

        if (Runner.TryFindObject(ghostId, out NetworkObject ghostObj))
        {
            closestCandidates.Add(null);
            currentTarget.Add(null);
            ghostAI.Add(ghostObj.GetComponent<GhostAI>());
        }
    }

    protected override IEnumerator OnStageEnd()
    {
        closestCandidates.Clear();
        currentTarget.Clear();

        if (HasStateAuthority)
            foreach (GhostAI ghost in ghostAI)
                Runner.Despawn(ghost.GhostNetObj);  // 고스트 Despawn

        ghostAI.Clear();

        yield break;
    }
    #endregion

    #region Ghost AI
    /// <summary>
    /// 각 고스트가 가장 가까운 플레이어를 추적 대상으로 설정하는 함수.
    /// FixedUpdate 등에서 주기적으로 호출하여 타겟 갱신.
    /// </summary>
    private void UpdateGhostTarget()
    {
        // 각 고스트 AI마다 가장 가까운 플레이어 탐색
        for (int i = 0; i < ghostAI.Count; i++)
        {
            float closestDist = float.MaxValue;

            // 모든 생존 플레이어를 순회
            foreach (var player in AlivePlayers)
            {
                float dist = Vector3.Distance(ghostAI[i].transform.position, player.Value.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    // 가장 가까운 플레이어를 현재 후보로 설정
                    closestCandidates[i] = player.Value;
                }
            }
        }

        // 계산된 타겟 후보를 현재 타겟으로 적용
        for (int i = 0; i < closestCandidates.Count; i++)
            currentTarget[i] = closestCandidates[i];
    }

    /// <summary>
    /// 각 고스트를 해당 타겟 위치로 이동시키는 함수.
    /// FixedUpdate 등에서 호출하여 이동 반영.
    /// </summary>
    private void MoveGhostToTarget()
    {
        if (currentTarget == null)
            return;

        // 각 고스트가 자신의 타겟 방향으로 NavMesh 이동 수행
        for (int i = 0; i < ghostAI.Count; i++)
            if (currentTarget[i] != null)
                ghostAI[i].Nav.SetDestination(currentTarget[i].transform.position);
    }
    #endregion
}