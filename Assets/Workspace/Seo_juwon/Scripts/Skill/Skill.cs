using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Skill
{
    string SkillName { get; }
    void Execute(Character user); // 스킬 사용 시 실행할 메서드
}
