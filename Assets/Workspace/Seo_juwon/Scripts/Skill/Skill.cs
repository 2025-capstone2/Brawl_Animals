using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float duration; //스킬 지속 시간
    public float cooltime = 3f; // 쿨타임
    public abstract void Execute(Character user);
}