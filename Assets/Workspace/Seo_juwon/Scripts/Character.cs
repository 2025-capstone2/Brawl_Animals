using Fusion;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public string characterName;
    public Animator animator;
    public int maxHp = 1000;
    public int currentHp = 1000;
    private float lastSkill = -10f;
    public float AttackCooltime = 1f;
    public float AttackRange = 2f;
    public int AttackDamage = 20;
    private float lastAttack = -10f;
    private bool isUsingSkill = false;
    public Skill skill;
    public Transform firePoint;

    private void Start()
    {
        currentHp = maxHp;

        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
                Debug.LogError("[Character] firePoint is null!");
        }

        Debug.Log($"{characterName} HP: {currentHp}");
    }

    private void Update()
    {
        if (transform.position.y < -10f)
        {
            Debug.Log($"{characterName} 맵 아래로 떨어져 사망");
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        Debug.Log($"{characterName} 피해 입음: {amount}, 남은 HP: {currentHp}");

        if (currentHp <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log($"{characterName} 사망");
        gameObject.SetActive(false);
    }

    public void UseSkill()
    {
        if (!HasInputAuthority) return;

        if (skill == null || isUsingSkill)
        {
            Debug.LogWarning("스킬이 없거나 사용 중");
            return;
        }

        if (!CanSkill())
            return;

        RPC_ExecuteSkill();
        lastSkill = Time.time;
    }

    private bool CanSkill()
    {
        if (Time.time < lastSkill + skill.cooltime)
        {
            float remain = (lastSkill + skill.cooltime) - Time.time;
            Debug.Log($"스킬 쿨타임 {remain:F1}초");
            return false;
        }

        return true;
    }

    public void TryAttack()
    {
        if (!HasInputAuthority) return;

        if (isUsingSkill || !CanAttack())
            return;

        RPC_PlayAttackAnimation();
        ExecuteAttack();
        lastAttack = Time.time;

        Debug.Log($"{characterName} 공격함");
    }

    private bool CanAttack()
    {
        if (Time.time < lastAttack + AttackCooltime)
        {
            float remain = (lastAttack + AttackCooltime) - Time.time;
            Debug.Log($"공격 쿨타임: {remain:F1}초 남음");
            return false;
        }

        return true;
    }

    private void ExecuteAttack()
    {
        if (firePoint == null) return;

        Collider[] hits = Physics.OverlapSphere(firePoint.position, AttackRange);

        foreach (var hit in hits)
        {
            Character enemy = hit.GetComponentInParent<Character>();
            if (enemy != null && enemy != this)
            {
                enemy.TakeDamage(AttackDamage);
                Debug.Log($"{enemy.characterName}: {AttackDamage} 피해 입음");
            }
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_ExecuteSkill()
    {
        if (skill == null || firePoint == null)
        {
            Debug.LogWarning("[Character] Skill 또는 firePoint 누락");
            return;
        }
        if (HasStateAuthority)
        {
            skill.Execute(this, Runner, Object.InputAuthority);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, AttackRange);
        }
    }
}