using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPoints : MonoBehaviour
{
    public int points;
    public int kills;
    public Text coinsText;
    public Text killCount;
    private int finalPoints;
    public int killPointMultiplier;

    public void Start()
    {
        points = 20;
    }

    public void AddPoints(int i)
    {
        points += i;
    }

    public void LosePoints(int i)
    {
        points -= i;
    }

    public int GetPoints()
    {
        return points;
    }

    public void gainKills()
    {
        kills++;
    }

    public int getKills()
    {
        return kills;
    }

    public void Update()
    {
        //totalPoints.text = countingPoints.ToString();
        if (points >= 999)
        {
            points = 999;
        }
        coinsText.text = points.ToString();
    }

    public void FinalScore()
    {
        finalPoints = points + (kills * killPointMultiplier);
    }
    public float GetFinalScore()
    {
        return finalPoints;
    }
}