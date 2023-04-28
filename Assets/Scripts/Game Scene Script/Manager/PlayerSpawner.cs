using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerStats player;
    //�÷��̾���� ������ų ��ġ���� ������ ��ũ��Ʈ
    public Transform[] playerSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        RandomSelectSpawnPoint();
    }
    void RandomSelectSpawnPoint()
    {
        int number = Random.Range(0, playerSpawnPoint.Length);
        player.transform.position = playerSpawnPoint[number].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
