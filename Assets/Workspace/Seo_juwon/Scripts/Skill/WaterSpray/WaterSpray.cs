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

        runner.Spawn(
            sprayPrefab.GetComponent<NetworkObject>(),
            user.firePoint.position,
            user.firePoint.rotation,
            authority,
            (runner, obj) =>
            {
                var controller = obj.GetComponent<WaterSprayController>();
                if (controller != null)
                {
                    controller.owner = user.gameObject;
                    controller.followTarget = user.firePoint;
                }
            }
        );
    }
}