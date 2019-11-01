using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawn : MonoBehaviour
{
    public GameObject scoreManager;
    public GameObject aiGameObject;
    public Transform[] spawnLocation;
    private int previousSpawn;
    private int index;

    [Header("shopTimer")]
    public GameObject shoptimerObject;
    public GameObject waveObj;
    public Transform upPos;
    public Transform downPos;
    public Text shopTimer;
    private float lerpUpProgress;
    private float lerpDownProgress;
    public float lerpSpeed;
    public Text WaveCountText;
    public int waveCount = 0;


    public int spawnAmount;
    [Header("increase enemies each wave")]

    private int waveMultiplierStartValue = 1;
    [Tooltip("set true to increase enemy spawn amount each wave")]
    public bool waveMultiplierOn;
    [Tooltip("increases the amount spawned each wave")]
    public int multiplyAmount;

    [Header("Wave Timer")]
    public float timeBetweenSpawn;
    public float startTimeBetweenWaves;
    public float addedTimeBetweenWaves;

    public float waveTimer;
    private int childNumber;

    [Tooltip("will be automatically turned on for the time between waves")]
    public bool shopOpen;

    [Header("boss")]
    [Tooltip("will spawn the boss if on(spawns on last wave)")]
    public bool bossOn;
    public GameObject boss;
    [Tooltip("where the boss will spawn")]
    public Transform bossSpawnLocation;


    // Start is called before the first frame update
    void Start()
    {
        waveTimer = 0;
        previousSpawn = Random.Range(0, spawnLocation.Length);
    }

    // Update is called once per frame
    void Update()
    {
        shopTimer.text = waveTimer.ToString("#.0");
        waveTimer -= Time.deltaTime;
        //if there is no children of the spawnmaster then start spawning enemies
        if (waveCount == scoreManager.GetComponent<ScoreManager>().finalWaveNumber + 1 || scoreManager.GetComponent<ScoreManager>().gameLost)
        {
            shoptimerObject.SetActive(false);
            waveObj.SetActive(false);
        }
        if (numberOfChildren() == 0)
        {
            if(waveTimer > 0)
            {
                if(lerpUpProgress < 1)
                {
                    lerpDownProgress = 0;
                    shopTimer.gameObject.SetActive(true);
                    lerpUpProgress += Time.deltaTime * lerpSpeed;
                    shoptimerObject.transform.position = Vector3.Lerp(downPos.position, upPos.position, lerpUpProgress);
                }
                //shoptimerObject.SetActive(true);
                shopOpen = true;
            }
            if(waveTimer < 0)
            {

                //if (shoptimerObject.transform.position == downPos.position)
                //{
                //    shoptimerObject.SetActive(false);
                //}
                shopOpen = false;
                waveCount++;

                startTimeBetweenWaves = startTimeBetweenWaves + addedTimeBetweenWaves;
                if(startTimeBetweenWaves < 0)
                {
                    startTimeBetweenWaves = 0;
                }
                StartCoroutine(spawnDelay());
                waveTimer = 0;
            }
        }
        else
        {
            if (lerpDownProgress < 1)
            {
                lerpUpProgress = 0;
                shopTimer.gameObject.SetActive(false);
                lerpDownProgress += Time.deltaTime * lerpSpeed;
                shoptimerObject.transform.position = Vector3.Lerp(upPos.position, downPos.position, lerpDownProgress);
            }
            waveTimer = startTimeBetweenWaves;
        }

        WaveCountText.text = waveCount.ToString() + "/" + scoreManager.GetComponent<ScoreManager>().finalWaveNumber;
    }

    //counts how many children there are
    int numberOfChildren()
    {
        for(int i = 0; i < spawnLocation.Length;i++)
        {
            childNumber = this.transform.childCount;
        }
        return childNumber;
    }

    //delays each spawn of enemies by a set amount of time
    IEnumerator spawnDelay()
    {
        if(scoreManager != null && scoreManager.GetComponent<ScoreManager>().finalWaveNumber == waveCount && bossOn)
        {
            Instantiate(boss, bossSpawnLocation.position, bossSpawnLocation.rotation,this.transform);
        }
        for(int i = 0; i < (spawnAmount * waveMultiplierStartValue); i++)
        {
            do { FindingIndex(); }
            while (index == previousSpawn);

            Instantiate(aiGameObject, spawnLocation[index].position, Quaternion.Euler(0, 0, 0), this.transform);
            previousSpawn = index;
            yield return new WaitForSeconds(timeBetweenSpawn);
        }
        if (waveMultiplierOn == true)
        {
            waveMultiplierStartValue = waveMultiplierStartValue + multiplyAmount;
        }
    }

    int FindingIndex()
    {
        index = Random.Range(0, spawnLocation.Length);
        return index;
    }
}
