using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    public int points = 0;

    public void AddPoints(int i)
    {
        points += i;
    }

    public int GetPoints()
    {
        return points;
    }

    private void Update()
    {
        
    }
}