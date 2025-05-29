using UnityEngine;
/// <summary>
/// [ScriptableObject 기반의 예시 스킬 클래스]
/// </summary>
[CreateAssetMenu(menuName = "Skill/WaterSpray")]
public class WaterSpray : Skill
{
    [Header("Water Spray Settings")]
    /// <summary>
    /// 인스펙터에서 연결하는 particle prefab 필요
    /// 발동 시 이 prefab이 firePoint에 붙는다.
    /// firepoint는 캐릭터 손 위치나 발사 위치. player 자식 컴포넌트로 추가
    /// Particle System(또는 스킬이 될 prefab) + [Skill]Controller를 포함해야 함
    /// </summary>
    public GameObject sprayPrefab;
    public float lifetime = 3f; // 파티클 시각적 유지 시간 (duration은 스킬 자체 지속 시간)

    public override void Execute(Character user)
    {
        if (sprayPrefab == null || user == null || user.firePoint == null)
        {
            Debug.LogWarning("스킬 실행 실패");
            return;
        }

        GameObject spray = Instantiate(sprayPrefab, user.firePoint.position, user.firePoint.rotation);
        spray.transform.SetParent(user.firePoint); //캐릭터가 회전할 때 같이 움직이도록 함

        // 소유자 정보 넘겨서 본인은 데미지를 받지 않게 함
        var controller = spray.GetComponent<WaterSprayController>();
        if (controller != null)
        {
            controller.owner = user.gameObject;
        }

        Destroy(spray, lifetime); // 일정 시간 후 파괴
    }
}