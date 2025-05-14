using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveInput;
    public NetworkButtons buttons;
    public NetworkBool skill;
    public NetworkBool attack;
}