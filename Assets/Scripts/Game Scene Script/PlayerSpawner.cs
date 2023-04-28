using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerStats player;
    //플레이어들을 스폰시킬 위치값을 저장할 스크립트
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
