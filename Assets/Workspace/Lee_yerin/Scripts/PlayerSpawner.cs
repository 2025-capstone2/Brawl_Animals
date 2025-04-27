using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개발자: 이예린
/// 플레이어를 스폰하는 클래스
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    [ContextMenu("Spawn")]
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        }

        Debug.Log("PlayerJoined");
    }
}
