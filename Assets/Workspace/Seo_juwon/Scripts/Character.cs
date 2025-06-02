using Fusion;
using UnityEngine;
using System.Collections;

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
    public bool isUsingSkill = false;
    public Skill skill;
    public Transform firePoint;
    public StageManager currentStage;
    [Header("상태 변수")]
    private int skillHitCount = 0;
    private float lastSkillHitTime = -10f;
    private bool isStunned = false;

    private void Start()
    {
        currentHp = maxHp;

        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
                Debug.LogError("[Character] firePoint is null!");
        }
        Debug.Log($"[UseSkill.STARTGAME] Skill 타입: {skill?.GetType().Name}");
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
        //죽으면 오브젝트 사라짐
        gameObject.SetActive(false);
        currentStage.AlivePlayers.Remove(this); // 플레이어 사망 시 스테이지 생존자 리스트에서 삭제
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

    public void UseSkill()
    {
        Debug.Log($"[UseSkill].UseSkill Skill 타입: {skill?.GetType().Name}");
        if (!HasInputAuthority) return;
        if (!CanSkill()) return;
        isUsingSkill = true;
        lastSkill = Time.time;
        if (skill is Squid)
            RPC_RequestSquidSkill();
        else
            RPC_RequestSkill();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestSkill()
    {
        Debug.Log("RPC_RequestSquidSkill 호출됨");
        RPC_ExecuteSkill();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ExecuteSkill()
    {
        if (skill == null || firePoint == null)
        {
            Debug.LogWarning("[Character] Skill 또는 firePoint 누락");
            return;
        }
        skill.Execute(this, Runner, Object.InputAuthority);
    }

    public void OnSkillHit(int damage)
    {
        TakeDamage(damage);

        if (isStunned)
            return;

        skillHitCount++;
        lastSkillHitTime = Time.time;

        Debug.Log($"{characterName} 스킬 피격 누적: {skillHitCount}");

        if (skillHitCount >= 8)
            StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;
        animator.SetTrigger("Spin");

        Debug.Log($"{characterName} 기절!");

        yield return new WaitForSeconds(1f);

        isStunned = false;
        skillHitCount = 0;

        Debug.Log($"{characterName} 기절 해제!");
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSquidSkill()
    {
        Debug.Log("[RequestSquidSkill]RPC_ExecuteSquidSkill 호출");
        RPC_ExecuteSquidSkill();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ExecuteSquidSkill()
    {
        Debug.Log("RPC_ExecuteSquidSkill 호출됨");

        if (!HasInputAuthority)
        {
            Debug.Log("HasInputAuthority == false → StartBlind 시도");
            SquidUI.Instance?.StartBlind();
        }
        else
        {
            Debug.Log("내 캐릭터이므로 Blind 패스");
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