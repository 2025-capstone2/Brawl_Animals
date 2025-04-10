using UnityEngine;

public class WaterSpray : Skill
{
    private GameObject sprayPrefab;

    public WaterSpray(GameObject prefab)
    {
        sprayPrefab = prefab;
    }

    public string SkillName => "WaterSpray";

    public void Execute(Character user)
    {
        if (sprayPrefab != null)
        {
            GameObject spray = Object.Instantiate(sprayPrefab, user.firePoint.position, user.firePoint.rotation);
            spray.transform.SetParent(user.firePoint);

            // 발사자 정보 전달
            WaterSprayController controller = spray.GetComponent<WaterSprayController>();
            if (controller != null)
            {
                controller.owner = user.gameObject;
            }
        }
        else
        {
            Debug.LogError("Water spray prefab is null!");
        }
    }
}