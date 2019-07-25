using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject EnemyMain;
    public GameObject EnemyHolder;
    public GameObject EnemySpawner;

    /// <summary>
    /// This is called at the first frame
    /// </summary>
    private void Start()
    {
        
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemy(EnemySpawner);
        }
    }

    /// <summary>
    /// spawns an enemy at a location
    /// </summary>
    /// <param name="spawner">Parent object of the spawner</param>
    private void SpawnEnemy(GameObject spawner)
    {
        GameObject InstEnemy;
        InstEnemy = Instantiate(EnemyMain, new Vector3(-4,1,0), Quaternion.Euler(0,0,0), spawner.transform);
        InstEnemy.transform.parent = EnemyHolder.transform;
    }
}
