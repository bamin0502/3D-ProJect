using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField] int _monsterCount = 0;     // 현재 몬스터 수
    [SerializeField] int _keepMonsterCount = 0; // 월드 상에 유지되야 하는 몬스터 수
    int _reserveCount = 0;  // ReserveSpawn 코루틴 함수 당 곧 생성할 몬스터 수

    [SerializeField] Vector3 _spawnPos;
    [SerializeField] float _spawnRadius = 15.0f;
    [SerializeField] float _spawnTime = 5.0f;

    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount += count; }


    void Start()
    {
        //졸려서 주석있는 건 뒤에 할예정
        //Managers.Game.OnSpawnEvent -= AddMonsterCount; 
        //Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine(ReserveSpawn());
        }
        if (Time.timeScale == 0)
        {
            return;
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        //GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");

        Vector3 randPos;
        //NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            NavMeshPath path = new NavMeshPath();
            //if (nma.CalculatePath(randPos, path))
                break;
        }

        //obj.transform.position = randPos;
        _reserveCount--;
    }
}
