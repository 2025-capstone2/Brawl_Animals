using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(menuName = "Skill/Rooster")]
public class Rooster : Skill
{
    public int damage = 10;
    public float hitInterval = 0.1f;
    public int hitCount = 6;
    public float hitRadius = 1.5f;
    public override void Execute(Character user, NetworkRunner runner, PlayerRef authority)
    {
        user.StartCoroutine(HitRoutine(user));
    }

    private IEnumerator HitRoutine(Character user)
    {
        if (user.animator != null)
        {
            for (int i = 0; i < 3; i++)
            {
                user.animator.SetTrigger("Skill");
                yield return null;
                while (!user.animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
                    yield return null;
                float animLength = user.animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animLength);
            }
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider[] hits = Physics.OverlapSphere(user.firePoint.position, hitRadius);

            foreach (var hit in hits)
            {
                Character enemy = hit.GetComponentInParent<Character>();
                if (enemy != null && enemy != user)
                {
                    enemy.OnSkillHit(damage);
                }
            }

            yield return new WaitForSeconds(hitInterval);
        }

        if (user.animator != null)
            user.animator.speed = 1f;
    }
    
}
