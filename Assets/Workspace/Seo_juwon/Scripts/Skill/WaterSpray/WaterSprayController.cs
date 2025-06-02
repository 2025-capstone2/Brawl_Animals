using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 데미지 크기, 피격 대상 관리
/// </summary>
public class WaterSprayController : MonoBehaviour
{
    public GameObject owner; // 발사자 캐릭터

    private float damageCooltime = 0.5f; //한번 데미지를 입은 뒤 연속적으로 피해를 입지 않기 위해
    private Dictionary<GameObject, float> lastHitTimes = new Dictionary<GameObject, float>();

    private void OnParticleCollision(GameObject other)
    {
        if (other == owner)
        {
            return; // 자기가 자기를 맞추면 무시
        }

        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            if (!lastHitTimes.ContainsKey(other) || Time.time - lastHitTimes[other] >= damageCooltime)
            {
                character.TakeDamage(50);
                lastHitTimes[other] = Time.time;
                Debug.Log($"물 스프레이 명중! {other.name} 피해 50");
            }
        }
    }
}