using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 개발자: 이예린
/// 
/// 무덤 맵에서 사용되는 고스트 AI 구현
/// </summary>
public class GhostAI : NetworkBehaviour
{
    [Header("Ghost AI NPC")]
    [SerializeField] NetworkObject ghostNetObj;
    public NetworkObject GhostNetObj => ghostNetObj;
    [SerializeField] NavMeshAgent nav;
    public NavMeshAgent Nav => nav;
    public Character currentTarget;

    [Header("Chack Player Die")]
    [SerializeField] LayerMask PlayerCheck;
    [SerializeField] GameObject tombObj;    // 무덤 오브젝트
    public List<NetworkObject> TombList { get; private set; }

    #region Unityb Event
    private void Start()
    {
        TombList = new List<NetworkObject>();
    }
    #endregion

    #region Catched Player Chracter
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerCheck.Contain(other.gameObject.layer))    // Trigger된 오브젝트의 레이어가 플레이어일 경우
        {
            if (currentTarget == null)
                return;

            Transform chracTrans = currentTarget.transform;
            currentTarget.Die();
            currentTarget = null;
            NetworkObject tomb = Runner.Spawn(tombObj, chracTrans.position, Quaternion.identity);    // 플레이어 사망 위치에 무덤 생성
            TombList.Add(tomb);
        }
    }
    #endregion

    /// <summary>
    /// 생성해둔 모든 무덤 Despawn 하는 메서드
    /// </summary>
    /// <returns></returns>
    public bool DespawnTombs()
    {
        foreach (NetworkObject tomb in TombList)
            Runner.Despawn(tomb);

        return true;
    }
}