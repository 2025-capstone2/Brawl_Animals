using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(menuName = "Skill/Rooster")]
public class Rooster : Skill
{
    public int damage = 10;
    public float hitInterval = 1f;
    public int hitCount = 6;
    public float hitRadius = 1.5f;
    public override void Execute(Character user, NetworkRunner runner, PlayerRef authority)
    {
        user.StartCoroutine(HitRoutine(user));
    }

    private IEnumerator HitRoutine(Character user)
    {
        user.isUsingSkill = true; // 스킬 중임 표시

        for (int i = 0; i < hitCount; i++)
        {
            // 애니메이션 트리거 (필요하면 제거 가능)
            if (user.animator != null)
                user.animator.SetTrigger("Skill");

            // 주변 콜라이더 체크 및 데미지
            Collider[] hits = Physics.OverlapSphere(user.firePoint.position, hitRadius);
            foreach (var hit in hits)
            {
                Character enemy = hit.GetComponentInParent<Character>();
                if (enemy != null && enemy != user)
                    enemy.OnSkillHit(damage);
            }

            yield return new WaitForSeconds(hitInterval); // 0.5초 간격
        }

        user.isUsingSkill = false; // 스킬 종료
    }
    
}
