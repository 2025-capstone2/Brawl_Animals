using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHp = 1000;
    public int currentHp = 1000;

    public Skill skill;
    public Transform firePoint;

    private void Start()
    {
        //시작할 때 hp는 max
        currentHp = maxHp;
        Debug.Log($"{characterName} HP: {currentHp}");
    }

    //피해를 입었을 때
    public void TakeDamage(int amount)
    {
        //현재 hp에서 amount만큼 깎임
        currentHp -= amount;
        Debug.Log($"{characterName} 피해 입음: {amount}, 남은 HP: {currentHp}");
        //hp가 0이 되면 사망
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{characterName} 사망");
        //죽으면 오브젝트 사라짐
        gameObject.SetActive(false);
    }
    //맵 아래로 떨어졌을 때
    private void Update()
    {
        if (transform.position.y < -10f)
        {
            Debug.Log($"{characterName} 맵 아래로 떨어져 사망");
            Die();
        }
    }
    public void UseSkill()
    {
        skill?.Execute(this);
    }
}