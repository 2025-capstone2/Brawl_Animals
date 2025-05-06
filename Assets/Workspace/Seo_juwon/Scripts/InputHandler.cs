using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 입력을 수신, 입력값 전달
/// PlayerPrefab에 붙이고, Spawn된 후 Runner에 스스로 등록함
/// </summary>
public class InputHandler :  NetworkBehaviour, INetworkRunnerCallbacks
{
    private Vector2 moveInput;
    private NetworkObject localPlayer;
    public void SetControlledPlayer(NetworkObject player)
    {
        localPlayer = player;
    }

    // PlayerInput에서 호출됨
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    // Fusion이 이 플레이어의 입력을 요청할 때 호출됨
    // 네트워크 입력 데이터로 moveInput 전달
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData
        {
            moveInput = moveInput
        };
        input.Set(data);
    }
    //네트워크에서 Spawn되었을 때 호출됨
     public override void Spawned()
    {
        if (HasInputAuthority)
        {
            Runner.ProvideInput = true;
            Runner.AddCallbacks(this);
            Debug.Log("[InputHandler] Runner에 등록됨");
        }
    }
    // 이 오브젝트가 네트워크에서 제거될 때 호출됨
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (HasInputAuthority)
        {
            runner.RemoveCallbacks(this);
        }
    }

    // Fusion 필수 콜백들 (빈 구현)
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}