using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject EnemyMain;
    public GameObject EnemyHolder;
    public GameObject EnemySpawner;

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemy(EnemySpawner);
        }
    }

    private void SpawnEnemy(GameObject spawner)
    {
        GameObject InstEnemy;
        InstEnemy = Instantiate(EnemyMain, new Vector3(-4,1,0), Quaternion.Euler(0,0,0), spawner.transform);
        InstEnemy.transform.parent = EnemyHolder.transform;
    }
}
