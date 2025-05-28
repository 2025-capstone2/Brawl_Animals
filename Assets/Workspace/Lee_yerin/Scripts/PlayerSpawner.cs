using Fusion;
using UnityEngine;

/// <summary>
/// 개발자: 이예린
/// 각 플레이어가 자기 오브젝트를 직접 생성하고 입력 권한을 가진다.
/// Fusion 권한 꼬임 방지를 위한 안전한 스폰 방식.
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"PlayerJoined 호출됨 Player: {player}, LocalPlayer: {Runner.LocalPlayer}");

        if (Runner.IsServer)
        {
            SpawnPlayer(player);
        }

        if (player == Runner.LocalPlayer)
        {
            Runner.ProvideInput = true;
        }
    }

    private void SpawnPlayer(PlayerRef player)
    {
        if (Runner == null || PlayerPrefab == null)
        {
            return;
        }

        var obj = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        Debug.Log($"spawn 완료 {obj.name} for {player} → InputAuthority: {obj.InputAuthority}, HasInputAuthority: {obj.HasInputAuthority}");
        Debug.LogWarning($" Player: {player}, LocalPlayer: {Runner.LocalPlayer}");
    }
}