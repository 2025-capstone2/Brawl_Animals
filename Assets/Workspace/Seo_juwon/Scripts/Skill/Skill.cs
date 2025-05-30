using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Skill : ScriptableObject {
    public float cooltime = 5f;
    public abstract void Execute(Character user, NetworkRunner runner, PlayerRef authority);
}