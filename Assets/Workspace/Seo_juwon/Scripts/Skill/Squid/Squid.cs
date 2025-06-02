using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Squid")]
public class Squid : Skill
{
    public float blindDelay = 1f;
    public float fadeDuration = 1f;

    public override void Execute(Character user, NetworkRunner runner, PlayerRef authority)
    {
        Debug.Log("Squid.Execute 호출됨");
        if (user.HasInputAuthority)
        {
            user.RPC_RequestSquidSkill();
        }
        else
        {
            Debug.LogWarning("[SquidSkill] 이 오브젝트에 InputAuthority가 없어 RPC 실행 안함");
        }
    }
}