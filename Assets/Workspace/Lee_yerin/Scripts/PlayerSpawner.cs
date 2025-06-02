using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

/// <summary>
/// 개발자: 이예린
/// 플레이어를 스폰하는 클래스
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Tooltip("첫번째 : 닭\n두번째 : 오징어\n")]
    public List<GameObject> PlayerPrefab = new();
    public Dictionary<PlayerRef, Animal> playerPickList;

    [SerializeField] public List<PlayerRef> pendingPlayers = new List<PlayerRef>();

    [ContextMenu("Spawn")]
    #region Spawn
    [ContextMenu("Spawn")]
    private NetworkObject SpawnPlayer(PlayerRef player, Transform spawnPoint)
    {
        try
        {
            if (Runner == null)
            {
                Debug.LogError("Runner가 설정되어 있지 않습니다.");
                return null;
            }

            if (PlayerPrefab == null)
            {
                Debug.LogError("Player Prefab이 설정되어 있지 않습니다.");
                return null;
            }

            GameObject chracter = PlayerPrefab[(int) playerPickList[player]];

            NetworkObject playerObj = Runner.Spawn(chracter, spawnPoint.position, Quaternion.identity, player);


            if (playerObj == null)
                Debug.Log("플레이어 생성 안 됨");

            // Fusion 내부에 등록 (클라이언트에서도 이 오브젝트 찾을 수 있도록)
            Runner.SetPlayerObject(player, playerObj);

            // 플레이어 오브젝트 HasInputAuthority 확인용 
            if (playerObj.HasInputAuthority)
            {
                Debug.Log("내가 조작할 수 있는 내 플레이어가 생성되었습니다.");
            }
            else
            {
                Debug.Log("이건 내가 조작할 수 있는 플레이어가 아닙니다.");
            }

            return playerObj;
        }
        catch (Exception e)
        {
            Debug.LogError("Runner.Spawn 중 예외 발생: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 게임 안에 플레이어 들어올 때 호출되는 메서드
    /// </summary>
    /// <param name="player"></param>
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log(player);
        if (Runner == null)
        {
            Debug.LogError("Runner가 설정되어 있지 않습니다.");
            return;
        }

        // Host만 Spawn 책임을 진다 권한 부여는 InputHandler에.
        /*if (Runner.IsServer)
        {
            pendingPlayers.Add(player); // 대기 등록
        }*/

        pendingPlayers.Add(player); // 대기 등록
    }

    /// <summary>
    /// 게임에서 플레이어 떠날 시 호출되는 메서드
    /// pendingPlayers 리스트에서 떠난 플레이어 삭제
    /// </summary>
    /// <param name="player">PlayerRef</param>
    public void PlayerLeft(PlayerRef player)
    {
        /*if (!Runner.IsServer)
            return;*/

        Debug.Log($"[Spawner] {player}가 떠났습니다. 리스트에서 제거합니다.");
        pendingPlayers.Remove(player);
    }

    /// <summary>
    /// 게임 안 모든 플레이어의 캐릭터 스폰하는 메서드
    /// </summary>
    /// <param name="spawnPoints">캐릭터 스폰할 위치 리스트</param>
    /// <returns></returns>
    public List<NetworkObject> SpawnAllPendingPlayers(List<Transform> spawnPoints)
    {
        if (!Runner.IsServer)
            return null;

        List<NetworkObject> playerCharacters = new();

        for (int i = 0; i < pendingPlayers.Count; i++)
        {
            playerCharacters.Add(SpawnPlayer(pendingPlayers[i], spawnPoints[i]));
        }

        return playerCharacters;
    }
    #endregion
}

public enum Animal
{
    Rooster,
    Squid
}