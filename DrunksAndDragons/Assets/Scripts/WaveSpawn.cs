using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawn : MonoBehaviour
{
    public GameObject aiGameObject;
    public Transform[] spawnLocation;

    public int spawnAmount;
    [Header("increase enemies each wave")]
    public bool waveMultiplierOn = false;
    public int waveMultiplierStartValue;
    public int multiplyAmount;

    [Header("Wave Timer")]
    public float timeBetweenSpawn;
    public float startTimeBetweenWaves;
    public float addedTimeBetweenWaves;

    private float timer;
    private int childNumber;



    // Start is called before the first frame update
    void Start()
    {
        timer = startTimeBetweenWaves;  
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(numberOfChildren() == 0)
        {
            if(timer < 0)
            {
                startTimeBetweenWaves = startTimeBetweenWaves + addedTimeBetweenWaves;
                if(startTimeBetweenWaves < 0)
                {
                    startTimeBetweenWaves = 0;
                }
                StartCoroutine(spawnDelay());
                timer = 0;
            }
        }
        else
        {
            timer = startTimeBetweenWaves;
        }
    }

    int numberOfChildren()
    {
        for(int i = 0; i < spawnLocation.Length;i++)
        {
            childNumber = this.transform.childCount;
        }
        return childNumber;
    }

    IEnumerator spawnDelay()
    {
        for(int i = 0; i < (spawnAmount * waveMultiplierStartValue); i++)
        {
            int index = Random.Range(0, spawnLocation.Length);

            Instantiate(aiGameObject, spawnLocation[index].position, Quaternion.Euler(0, 0, 0), this.transform);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
        if (waveMultiplierOn == true)
        {
            waveMultiplierStartValue = waveMultiplierStartValue + multiplyAmount;
        }
    }
}
