using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 스폰 위치 배열
    public GameObject[] monsterPrefabs; // 몬스터 프리팹 배열

    public float spawnInterval = 3f; // 몬스터 스폰 간격
    public float initialSpawnDelay = 2f; // 초기 스폰 지연 시간

    private List<GameObject> monsterPool; // 몬스터 오브젝트 풀
    private List<int> availableSpawnPoints; // 스폰 가능한 위치 인덱스 리스트

    private void Start()
    {
        // 몬스터 오브젝트 풀 초기화
        monsterPool = new List<GameObject>();

        // 몬스터를 초기에 전부 스폰
        SpawnAllMonsters();
        // 스폰 가능한 위치 인덱스 리스트 초기화
        availableSpawnPoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        // 초기 스폰 지연 시간 후 몬스터 스폰 시작
        Invoke(nameof(StartSpawning), initialSpawnDelay);
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnMonsters());
    }
    private void SpawnAllMonsters()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            // 몬스터를 오브젝트 풀에서 가져오거나 생성
            GameObject monster = GetOrCreateMonsterFromPool();

            // 몬스터를 스폰 위치에 스폰
            monster.transform.position = spawnPoint.position;
            monster.SetActive(true);
        }
    }
    private IEnumerator SpawnMonsters()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 스폰 가능한 위치가 있는지 확인
            if (availableSpawnPoints.Count == 0)
                continue;

            // 스폰 위치 인덱스를 랜덤하게 선택
            int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[spawnIndex];

            // 해당 스폰 위치에 몬스터가 이미 스폰되어 있는지 확인
            if (IsMonsterSpawnedAtPoint(spawnPointIndex))
                continue;

            // 몬스터를 오브젝트 풀에서 가져오거나 생성
            GameObject monster = GetOrCreateMonsterFromPool();

            // 몬스터를 선택된 스폰 위치에 스폰
            monster.transform.position = spawnPoints[spawnPointIndex].position;
            monster.SetActive(true);

            // 스폰된 위치 인덱스를 스폰 가능한 위치 인덱스 리스트에서 제거
            availableSpawnPoints.RemoveAt(spawnIndex);
        }
    }
    private bool IsMonsterSpawnedAtPoint(int spawnPointIndex)
    {
        // 해당 스폰 위치에 몬스터가 있는지 확인하는 로직을 구현
        // 이미 몬스터가 있다면 false를 반환하고, 없다면 true를 반환하여 스폰을 진행

        bool hasMonster = false;

        // 해당 스폰 위치에 몬스터가 있는지 검사
        // 예시에서는 monsterPool 리스트를 사용하여 해당 위치의 몬스터 유무를 판단
        GameObject monster = monsterPool.Find(m => m != null && m.activeSelf && m.transform.position == spawnPoints[spawnPointIndex].position);
        if (monster == null)
        {
            hasMonster = true;
        }

        return hasMonster;
    }


    // 몬스터를 오브젝트 풀에서 가져오거나 생성
    private GameObject GetOrCreateMonsterFromPool()
    {
        GameObject monster = monsterPool.Find(m => !m.activeSelf);
        if (monster == null)
        {
            // 모든 몬스터 프리팹을 순회하며 오브젝트 풀에 추가
            foreach (GameObject monsterPrefab in monsterPrefabs)
            {
                GameObject newMonster = Instantiate(monsterPrefab);
                newMonster.SetActive(false);
                monsterPool.Add(newMonster);
            }
            // 다시 몬스터를 가져옴
            monster = monsterPool.Find(m => !m.activeSelf);
        }
        return monster;
    }
}

