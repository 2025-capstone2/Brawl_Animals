using UnityEngine;
using System.Collections.Generic;

public class WaterSprayController : MonoBehaviour
{
    public GameObject owner; // 발사자 캐릭터

    private float damageCooldown = 0.5f;
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
            if (!lastHitTimes.ContainsKey(other) || Time.time - lastHitTimes[other] >= damageCooldown)
            {
                character.TakeDamage(50);
                lastHitTimes[other] = Time.time;
                Debug.Log($"물 스프레이 명중! {other.name} 피해 50");
            }
        }
    }
}