using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSpawnData
    {
        // Player1 or Player2 프리팹
        public GameObject prefab;
        // 해당 플레이어의 시작 위치
        public Transform spawnPoint;
        // 해당 플레이어가 사용할 스킬
        public Skill skill;
    }
    // 파티클 프리팹
    public GameObject waterSprayPrefab;

    public CharacterSpawnData[] charactersToSpawn;

    void Start()
    {
        foreach (CharacterSpawnData data in charactersToSpawn)
        {
            SpawnCharacter(data);
        }
    }

    void SpawnCharacter(CharacterSpawnData data)
    {
        GameObject clone = Instantiate(data.prefab, data.spawnPoint.position, data.spawnPoint.rotation);
        Character character = clone.GetComponent<Character>();

        if (character == null)
        {
            Debug.LogError("캐릭터 프리팹에 Character 스크립트가 없음");
            return;
        }

        character.skill = data.skill;
        character.firePoint = data.spawnPoint;
    }
}