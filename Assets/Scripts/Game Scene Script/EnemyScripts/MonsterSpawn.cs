using System.Collections;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public float spawnInterval = 3f; // 스폰 간격
    public int maxSpawnCount = 5; // 최대 스폰 수
    public float spawnAreaRadius = 10f; // 스폰 영역 반지름

    private int currentSpawnCount = 0; // 현재 스폰된 몬스터 수

    private void Start()
    {
        // 일정 시간마다 몬스터 스폰을 반복하는 코루틴 시작
        StartCoroutine(SpawnMonsters());
    }

    private IEnumerator SpawnMonsters()
    {
        while (currentSpawnCount < maxSpawnCount)
        {
            // 스폰 위치 계산
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // 몬스터 생성
            GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

            // 몬스터 생성 시 처리할 로직 추가 (예: 스폰된 몬스터 수 증가, 몬스터 초기화 등)

            currentSpawnCount++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 스폰 영역 내의 랜덤한 위치를 계산
        Vector2 randomCircle = Random.insideUnitCircle * spawnAreaRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // 스폰 위치를 구성하는 y 좌표를 조정할 수도 있습니다.
        // spawnPosition.y = 0f;

        return spawnPosition;
    }
}
