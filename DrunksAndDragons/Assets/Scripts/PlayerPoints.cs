using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    private int points;
    // Start is called before the first frame update
    void Start()
    {
        points = 0; 
    }

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
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            int p = Random.Range(1, 20);
            AddPoints(p);
        }
    }
}