using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPoints : MonoBehaviour
{
    public int points;
    public int totalCoins;
    public int kills;
    public Text coinsText;
    private int finalPoints;
    public int killPointMultiplier;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public void Start()
    {
        points = 20;
        totalCoins = points;
    }

    /// <summary>
    /// Adds points to the player
    /// </summary>
    /// <param name="i">Amount of points added</param>
    public void AddPoints(int i)
    {
        points += i;
        totalCoins += i;
    }

    /// <summary>
    /// Player loses points
    /// </summary>
    /// <param name="i">the amount of points being lost</param>
    public void LosePoints(int i)
    {
        points -= i;
    }

    /// <summary>
    /// Get the amount of points the player has
    /// </summary>
    /// <returns>The points</returns>
    public int GetPoints()
    {
        return points;
    }

    public int getTotalCoins()
    {
        return totalCoins;
    }

    /// <summary>
    /// Increases the amount of kills the player has
    /// </summary>
    public void gainKills()
    {
        kills++;
    }

    /// <summary>
    /// Returns the amount of kills the player has
    /// </summary>
    /// <returns>The kills of the player</returns>
    public int getKills()
    {
        return kills;
    }

    /// <summary>
    /// Update is called before each frame update
    /// </summary>
    public void Update()
    {
        //totalPoints.text = countingPoints.ToString();
        if (points >= 999)
        {
            points = 999;
        }
        if (coinsText != null) coinsText.text = points.ToString();
    }

    /// <summary>
    /// Calculate the final score of the player
    /// </summary>
    public void FinalScore()
    {
        finalPoints = points + (kills * killPointMultiplier);
    }
    /// <summary>
    /// Gets the final score of the player
    /// </summary>
    /// <returns>the amount of points the player has</returns>
    public float GetFinalScore()
    {
        return finalPoints;
    }
}