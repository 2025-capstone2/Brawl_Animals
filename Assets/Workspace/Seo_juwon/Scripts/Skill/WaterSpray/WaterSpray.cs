using UnityEngine;
using System.Collections;
using Fusion;

[CreateAssetMenu(menuName = "Skill/WaterSpray")]
public class WaterSpray : Skill
{
    public NetworkObject sprayPrefab;
    public float lifetime = 3f;

    public override void Execute(Character user, NetworkRunner runner, PlayerRef authority)
    {
        if (sprayPrefab == null || user == null || user.firePoint == null)
        {
            Debug.LogWarning("스킬 실행 실패");
            return;
        }

        NetworkObject prefabNetObj = sprayPrefab.GetComponent<NetworkObject>();

        if (prefabNetObj == null)
        {
            Debug.LogWarning("NetworkObject 컴포넌트가 sprayPrefab에 없음");
            return;
        }

        NetworkObject sprayObj = runner.Spawn(prefabNetObj, user.firePoint.position, user.firePoint.rotation, authority);

        WaterSprayController controller = sprayObj.GetComponent<WaterSprayController>();
        if (controller != null)
        {
            controller.owner = user.gameObject;
            // controller에서 Despawn Coroutine 자동 실행
        }
    }
}