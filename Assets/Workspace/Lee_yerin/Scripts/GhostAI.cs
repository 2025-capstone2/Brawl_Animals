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
public class GhostAI : MonoBehaviour
{
    [Header("Ghost AI NPC")]
    [SerializeField] NetworkObject ghostNetObj;
    public NetworkObject GhostNetObj => ghostNetObj;
    [SerializeField] NavMeshAgent nav;
    public NavMeshAgent Nav => nav;
}
