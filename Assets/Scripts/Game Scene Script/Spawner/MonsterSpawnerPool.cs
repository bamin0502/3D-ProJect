using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField] int _monsterCount = 0;     // ���� ���� ��
    [SerializeField] int _keepMonsterCount = 0; // ���� �� �����Ǿ� �ϴ� ���� ��
    int _reserveCount = 0;  // ReserveSpawn �ڷ�ƾ �Լ� �� �� ������ ���� ��

    [SerializeField] Vector3 _spawnPos;
    [SerializeField] float _spawnRadius = 15.0f;
    [SerializeField] float _spawnTime = 5.0f;

    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount += count; }


    void Start()
    {
        //������ �ּ��ִ� �� �ڿ� �ҿ���
        //Managers.Game.OnSpawnEvent -= AddMonsterCount; 
        //Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine(ReserveSpawn());
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
