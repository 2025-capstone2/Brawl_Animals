using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 개발자: 이예린
/// 플레이어를 스폰하는 클래스
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    [ContextMenu("Spawn")]
    #region Spawn
    [ContextMenu("Spawn")]
    private void SpawnPlayer(PlayerRef player)
    {
        try
        {


            if (Runner == null)
            {
                Debug.LogError("Runner가 설정되어 있지 않습니다.");
                return;
            }

            if (PlayerPrefab == null)
            {
                Debug.LogError("Player Prefab이 설정되어 있지 않습니다.");
                return;
            }

            NetworkObject playerObj = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);


            if (playerObj == null)
                Debug.Log("플레이어 생성 안 됨");

            // 플레이어 오브젝트 HasInputAuthority 확인용 
            if (playerObj.HasInputAuthority)
            {
                Debug.Log("내가 조작할 수 있는 내 플레이어가 생성되었습니다.");
            }
            else
            {
                Debug.Log("이건 내가 조작할 수 있는 플레이어가 아닙니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Runner.Spawn 중 예외 발생: " + e.Message);
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log(player);
        if (Runner == null)
        {
            Debug.LogError("Runner가 설정되어 있지 않습니다.");
            return;
        }

        // Host만 Spawn 책임을 진다 권한 부여는 InputHandler에.
        if (Runner.IsServer)
        {
            SpawnPlayer(player);
        }
        /*
        if (Runner.IsClient)
        {
            Runner.ProvideInput = true;
        }
        */
    }
    #endregion
}